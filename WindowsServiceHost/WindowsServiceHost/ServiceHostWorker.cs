using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using DKK.Commands;
using DKK.Messaging;
using DKK.POCOProvider;
using DKK.ServiceHostCommands;
using DKK.ServiceHostEvents;

namespace DKK.WindowsServiceHost
{
	[Serializable]
	internal sealed class ServiceHostWorker : MarshalByRefObject, IDisposable
	{
		[Serializable]
		private sealed class EnvironmentComponents
		{
			public string Environment { get; set; }
			public List<KeyValuePair<string, string>> Config = new List<KeyValuePair<string, string>>();
			public ICommandConsumer CommandConsumer { get; set; }
			public List<RunningServiceComponent> Components = new List<RunningServiceComponent>();
			public MessageBrokerConnection MessageBrokerConnection { get; set; }
			public IEventProducer EventProducer { get; set; }
		}

		private Dictionary<string, EnvironmentComponents> Environments;
		private string Process { get; }

		public string ConfigURL { get; set; }
		public IReloader Reloader { get; set; }

		public ServiceHostWorker()
		{
			this.Environments = new Dictionary<string, EnvironmentComponents>();
			this.Process = Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().CodeBase);
		}

		public void Start()
		{
			if (!System.Diagnostics.Debugger.IsAttached)
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_UnhandledException);

			if (string.IsNullOrWhiteSpace(this.ConfigURL))
				throw new ArgumentNullException(nameof(ConfigURL), "ConfigURL not supplied");

			this.LoadEnvironments();

			foreach (var environment in this.Environments.Keys)
				this.Start(environment);
		}

		private void Start(string environment)
		{
			var envConfig = this.Environments[environment];
			var eventProducer = envConfig.EventProducer;

			envConfig
					.Components
						.ForEach(rsc =>
						{
							try
							{
								this.PublishStateChange(eventProducer, environment, "Starting");
								rsc.Start(eventProducer);
								this.PublishStateChange(eventProducer, environment, "Started");
							}
							catch(Exception ex)
							{
								this.PublishStateChangeFailed(eventProducer, environment, "StartFailed", ex);
							}
						});
		}

		public void Stop()
		{
			foreach (var environment in this.Environments.Keys)
				this.Stop(environment);
		}

		private void Stop(string environment)
		{
			var envConfig = this.Environments[environment];
			var eventProducer = envConfig.EventProducer;

			envConfig
					.Components
						.ForEach(rsc =>
						{
							try
							{
								this.PublishStateChange(eventProducer, environment, "Stopping");
								rsc.Stop(eventProducer);
								this.PublishStateChange(eventProducer, environment, "Stopped");
							}
							catch (Exception ex)
							{
								this.PublishStateChangeFailed(eventProducer, environment, "StopFailed", ex);
							}
						});
		}

		public void Pause()
		{
			foreach (var environment in this.Environments.Keys)
				this.Pause(environment);
		}

		private void Pause(string environment)
		{
			var envConfig = this.Environments[environment];
			var eventProducer = envConfig.EventProducer;

			envConfig
					.Components
						.ForEach(rsc =>
						{
							try
							{
								this.PublishStateChange(eventProducer, environment, "Pausing");
								rsc.Pause(eventProducer);
								this.PublishStateChange(eventProducer, environment, "Paused");
							}
							catch (Exception ex)
							{
								this.PublishStateChangeFailed(eventProducer, environment, "PauseFailed", ex);
							}
						});
		}

		public void Continue()
		{
			foreach (var environment in this.Environments.Keys)
				this.Continue(environment);
		}

		private void Continue(string environment)
		{
			var envConfig = this.Environments[environment];
			var eventProducer = envConfig.EventProducer;

			envConfig
					.Components
						.ForEach(rsc =>
						{
							try
							{
								this.PublishStateChange(eventProducer, environment, "Continuing");
								rsc.Continue(eventProducer);
								this.PublishStateChange(eventProducer, environment, "Started");
							}
							catch (Exception ex)
							{
								this.PublishStateChangeFailed(eventProducer, environment, "ContinueFailed", ex);
							}
						});
		}

		private void LoadEnvironments()
		{
			var configClient = new SvcComponentConfig.SvcComponentConfigClient();
			var environments = configClient.GetEnvironments(new SvcComponentConfig.GetEnvironmentsRequest()).GetEnvironmentsResult;

			foreach (var environment in environments)
			{
				if (!this.Environments.ContainsKey(environment))
					this.Environments.Add(environment, new EnvironmentComponents() { Environment = environment });

				this.LoadConfiguration(environment);
			}
		}

		private void LoadConfiguration(string environment)
		{
			var envConfig = this.Environments[environment];
			var currConfig = envConfig.Config;

			var configClient = new SvcComponentConfig.SvcComponentConfigClient();
			var newConfig = configClient.GetEnvironmentConfig(new SvcComponentConfig.GetEnvironmentConfigRequest(environment)).GetEnvironmentConfigResult;

			if (!currConfig.SequenceEqual(newConfig))
			{
				envConfig.Config = newConfig;
				this.ManageMessageBrokerConnection(envConfig);
				this.ManageServiceComponents(envConfig);
			}
		}

		private void ManageMessageBrokerConnection(EnvironmentComponents envConfig)
		{
			var environment = envConfig.Environment;
			var rabbitEnv = envConfig.Config.Where(kvp => kvp.Key.StartsWith("RabbitMQ"));

			if (envConfig.MessageBrokerConnection == null)
			{
				envConfig.MessageBrokerConnection = new MessageBrokerConnection(rabbitEnv);
				envConfig.EventProducer = new EventProducer(envConfig.MessageBrokerConnection.Connection);
				CreateCommandListener(envConfig);
			}
			else
			{
				var existingConnection = envConfig.MessageBrokerConnection;

				var server = rabbitEnv.Single(kvp => kvp.Key == "RabbitMQServer").Value;
				var port = Convert.ToInt32(rabbitEnv.Single(kvp => kvp.Key == "RabbitMQPort").Value);
				var vHost = rabbitEnv.Single(kvp => kvp.Key == "RabbitMQVHost").Value;

				if (existingConnection.Server != server ||
					existingConnection.Port != port ||
					existingConnection.VHost != vHost)
				{
					RemoveCommandListener(envConfig);
					RemoveEventProducer(envConfig);

					existingConnection.Close();
					envConfig.MessageBrokerConnection = new MessageBrokerConnection(rabbitEnv);

					CreateEventProducer(envConfig);
					CreateCommandListener(envConfig);
				}
			}
		}

		private void CreateCommandListener(EnvironmentComponents envConfig)
		{
			if (envConfig.CommandConsumer != null)
				RemoveCommandListener(envConfig);

			envConfig.CommandConsumer = new CommandConsumer(envConfig.MessageBrokerConnection.Connection);

			#region ReloadFiles
			envConfig.CommandConsumer.RegisterCommandHandler<ReloadFiles>((props, cmd) =>
			{
				var reply = new ReloadFilesAck(cmd) as ICommand;

				try
				{
					Console.WriteLine("{0} Command received ({1})", cmd.GetType().Name, cmd.CorrelationId);
					this.Reloader.Reload();
				}
				catch
				{ reply = new ReloadFilesNack(cmd) as ICommand; }

				return reply;
			}); 
			#endregion

			#region ReloadConfiguration
			envConfig.CommandConsumer.RegisterCommandHandler<ReloadConfiguration>((props, cmd) =>
			{
				var reply = new ReloadConfigurationAck(cmd) as ICommand;
				var environment = envConfig.Environment;

				try
				{
					Console.WriteLine("{0} Command received ({1})", cmd.GetType().Name, cmd.CorrelationId);
					this.LoadConfiguration(environment);
				}
				catch
				{ reply = new ReloadConfigurationNack(cmd) as ICommand; }

				return reply;
			});
			#endregion

			#region Start Component
			envConfig.CommandConsumer.RegisterCommandHandler<StartComponent>((props, cmd) =>
			{
				var reply = new StartComponentAck(cmd) as ICommand;
				var environment = envConfig.Environment;

				try
				{
					RunningServiceComponent r = envConfig.Components.Single(rsc => rsc.Id == (cmd as StartComponent).ServiceComponentId);
					r.Start(envConfig.EventProducer);
				}
				catch
				{ reply = new StartComponentNack(cmd) as ICommand; }

				return reply;
			});
			#endregion

			#region Stop Component
			envConfig.CommandConsumer.RegisterCommandHandler<StopComponent>((props, cmd) =>
			{
				var reply = new StopComponentAck(cmd) as ICommand;
				var environment = envConfig.Environment;

				try
				{
					RunningServiceComponent r = envConfig.Components.Single(rsc => rsc.Id == (cmd as StopComponent).ServiceComponentId);
					r.Stop(envConfig.EventProducer);
				}
				catch
				{ reply = new StopComponentNack(cmd) as ICommand; }

				return reply;
			});
			#endregion

			#region Activate Component
			envConfig.CommandConsumer.RegisterCommandHandler<ActivateComponent>((props, cmd) =>
			{
				var reply = new ActivateComponentAck(cmd) as ICommand;
				var environment = envConfig.Environment;

				try
				{
					RunningServiceComponent r = envConfig.Components.Single(rsc => rsc.Id == (cmd as ActivateComponent).ServiceComponentId);
					r.Activate(envConfig.EventProducer);
				}
				catch
				{ reply = new ActivateComponentNack(cmd) as ICommand; }

				return reply;
			});
			#endregion

			#region Deactivate Component
			envConfig.CommandConsumer.RegisterCommandHandler<DeactivateComponent>((props, cmd) =>
			{
				var reply = new DeactivateComponentAck(cmd) as ICommand;
				var environment = envConfig.Environment;

				try
				{
					RunningServiceComponent r = envConfig.Components.Single(rsc => rsc.Id == (cmd as DeactivateComponent).ServiceComponentId);
					r.Deactivate(envConfig.EventProducer);
				}
				catch
				{ reply = new DeactivateComponentNack(cmd) as ICommand; }

				return reply;
			});
			#endregion

			#region Pause Component
			envConfig.CommandConsumer.RegisterCommandHandler<PauseComponent>((props, cmd) =>
			{
				var reply = new PauseComponentAck(cmd) as ICommand;
				var environment = envConfig.Environment;

				try
				{
					RunningServiceComponent r = envConfig.Components.Single(rsc => rsc.Id == (cmd as PauseComponent).ServiceComponentId);
					r.Pause(envConfig.EventProducer);
				}
				catch
				{ reply = new PauseComponentNack(cmd) as ICommand; }

				return reply;
			});
			#endregion

			#region Continue Component
			envConfig.CommandConsumer.RegisterCommandHandler<ContinueComponent>((props, cmd) =>
			{
				var reply = new ContinueComponentAck(cmd) as ICommand;
				var environment = envConfig.Environment;

				try
				{
					RunningServiceComponent r = envConfig.Components.Single(rsc => rsc.Id == (cmd as ContinueComponent).ServiceComponentId);
					r.Continue(envConfig.EventProducer);
				}
				catch
				{ reply = new ContinueComponentNack(cmd) as ICommand; }

				return reply;
			});
			#endregion

			UpdateCmdMsgQ(envConfig, envConfig.CommandConsumer.PublicationAddress.ToString());
		}

		private void RemoveCommandListener(EnvironmentComponents envConfig)
		{
			if (envConfig.CommandConsumer != null)
			{
				UpdateCmdMsgQ(envConfig, string.Empty);
				envConfig.CommandConsumer.Close();
				envConfig.CommandConsumer = null;
			}
		}

		private void CreateEventProducer(EnvironmentComponents envConfig)
		{
			if (envConfig.EventProducer != null)
				RemoveEventProducer(envConfig);

			envConfig.EventProducer = new EventProducer(envConfig.MessageBrokerConnection.Connection);
		}

		private void RemoveEventProducer(EnvironmentComponents envConfig)
		{
			if (envConfig.EventProducer != null)
			{
				envConfig.EventProducer.Close();
				envConfig.EventProducer = null;
			}
		}

		private void ManageServiceComponents(EnvironmentComponents envComponents)
		{
			var environment = envComponents.Environment;
			var mongoEnv = envComponents.Config.Where(kvp => kvp.Key.StartsWith("Mongo"));

			var provider = new ServiceHostProvider(mongoEnv);
			var sh = provider.Queryable().Single(q => q.Machine == Environment.MachineName);
			var configuredComponents = sh.Components;

			var configuredIds = configuredComponents.Select(c => c.Id);
			var existingIds = envComponents.Components.Select(c => c.Id);

			// newly configured components
			foreach (var newlyConfiguredId in configuredIds.Except(existingIds))
				envComponents.Components.Add(new RunningServiceComponent(environment, this.ConfigURL, configuredComponents.Single(c => c.Id == newlyConfiguredId), envComponents.Config));

			// newly removed components
			foreach (var newlyDeletedId in existingIds.Except(configuredIds))
			{
				RunningServiceComponent rsc = envComponents.Components.Single(c => c.Id == newlyDeletedId);
				rsc.Stop(envComponents.EventProducer);
				envComponents.Components.Remove(rsc);
			}

			// not new, not deleted, possibly updated
			foreach (var possiblyUpdatedId in configuredIds.Intersect(existingIds))
			{
				var rsc = envComponents.Components.Single(c => c.Id == possiblyUpdatedId);
				var sc = configuredComponents.Single(c => c.Id == possiblyUpdatedId);

				if (rsc.RowVersion != sc.RowVersion)
					rsc.Update(envComponents.EventProducer, sc);
			}
		}

		private void UpdateCmdMsgQ(EnvironmentComponents envConfig, string publicationAddress)
		{
			var mongoEnv = envConfig.Config.Where(kvp => kvp.Key.StartsWith("Mongo"));
			var provider = new ServiceHostProvider(mongoEnv);
			var sh = provider.Queryable().Single(q => q.Machine == Environment.MachineName);
			sh.CommandMessageQueue = publicationAddress;
			provider.Update(sh);
		}

		private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			DKKWindowsServiceHostEventSource.Log.Exception(e.ExceptionObject as Exception);
		}

		private void PublishStateChange(IEventProducer eventProducer, string environment, string state)
		{
			eventProducer.Publish(new ServiceHostStateChange() { Environment = environment, Process = this.Process, State = state });
		}

		private void PublishStateChangeFailed(IEventProducer eventProducer, string environment, string state, Exception ex)
		{
			eventProducer.Publish(new ServiceHostStateChangeFailed() { Environment = environment, Process = this.Process, State = state, Exception = ex });
		}

		#region MarshalByRefObject
		public override object InitializeLifetimeService()
		{
			var lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.SponsorshipTimeout = TimeSpan.Zero;
				lease.InitialLeaseTime = TimeSpan.Zero;
			}
			return lease;
		}
		#endregion

		#region IDisposable
		private bool disposed;
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					this.Stop();
					foreach (var envConfig in this.Environments.Values)
					{
						RemoveCommandListener(envConfig);
						RemoveEventProducer(envConfig);

						envConfig.Components.Clear();

						envConfig.MessageBrokerConnection.Close();
						envConfig.MessageBrokerConnection = null;
					}
					this.Environments.Clear();
				}
			}

			disposed = true;
		}
		#endregion
	}
}
