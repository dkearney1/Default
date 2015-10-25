using System;
using System.Collections.Generic;
using System.IO;
using DKK.Messaging;
using DKK.POCOs;
using DKK.ServiceHostEvents;
using DKK.WindowsServiceComponentInterface;

namespace DKK.WindowsServiceHost
{
	internal sealed class RunningServiceComponent
	{
		private string Environment { get; }
		private string ConfigURL { get; }
		private string Process { get; }
		private ServiceComponent ServiceComponentData { get; set; }
		private IEnumerable<KeyValuePair<string, string>> EnvironmentConfig { get; }

		private AppDomain WorkerDomain;
		private IWindowsServiceComponent IServiceComponent;

		public Guid Id => this.ServiceComponentData.Id;
		public int RowVersion => this.ServiceComponentData.RowVersion;

		public RunningServiceComponent(string environment, string configURL, ServiceComponent serviceComponent, IEnumerable<KeyValuePair<string, string>> envConfig)
		{
			this.Environment = environment;
			this.ConfigURL = configURL;
			this.ServiceComponentData = serviceComponent;
			this.EnvironmentConfig = envConfig;
			this.Process = Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().CodeBase);
		}

		public void Start(IEventProducer eventProducer)
		{
			if (!this.ServiceComponentData.IsActive)
				return;

			this.Activate(eventProducer);

			this.PublishStateChange(eventProducer, "Starting");

			try
			{
				if (this.IServiceComponent == null)
					this.IServiceComponent = this.WorkerDomain.CreateInstanceAndUnwrap(this.ServiceComponentData.Assembly, this.ServiceComponentData.Class) as IWindowsServiceComponent;

				this.IServiceComponent.ConfigURL = this.ConfigURL;
				this.IServiceComponent.Configuration = this.ServiceComponentData.Config;
				this.IServiceComponent.Environment = this.EnvironmentConfig;
				this.IServiceComponent.Start();

				this.PublishStateChange(eventProducer, "Started");
			}
			catch (Exception ex)
			{
				this.Deactivate(eventProducer);
				this.PublishStateChangeFailed(eventProducer, "StartFailed", ex);
				throw;
			}
		}

		public void Stop(IEventProducer eventProducer)
		{
			this.PublishStateChange(eventProducer, "Stopping");

			try
			{
				if (this.IServiceComponent != null)
					this.IServiceComponent.Stop();
				this.PublishStateChange(eventProducer, "Stopped");
			}
			catch (Exception ex)
			{
				this.PublishStateChangeFailed(eventProducer, "StopFailed", ex);
				throw;
			}
			finally
			{
				try
				{
					this.IServiceComponent.Dispose();
				}
				catch
				{ /* eating the exception */ }

				this.IServiceComponent = null;
			}
		}

		public void Pause(IEventProducer eventProducer)
		{
			this.PublishStateChange(eventProducer, "Pausing");

			try
			{
				if (this.IServiceComponent != null)
					this.IServiceComponent.Pause();
				this.PublishStateChange(eventProducer, "Paused");
			}
			catch (Exception ex)
			{
				this.PublishStateChangeFailed(eventProducer, "PauseFailed", ex);
				throw;
			}
		}

		public void Continue(IEventProducer eventProducer)
		{
			this.PublishStateChange(eventProducer, "Continuing");

			try
			{
				if (this.IServiceComponent != null)
					this.IServiceComponent.Pause();
				this.PublishStateChange(eventProducer, "Started");
			}
			catch (Exception ex)
			{
				this.PublishStateChangeFailed(eventProducer, "ContinueFailed", ex);
				throw;
			}
		}

		public void Activate(IEventProducer eventProducer)
		{
			this.PublishStateChange(eventProducer, "Activating");

			try
			{
				if (this.WorkerDomain == null)
				{
					var adSetup = new AppDomainSetup();
					adSetup.ApplicationName = this.ServiceComponentData.FriendlyName;
					this.WorkerDomain = AppDomain.CreateDomain(this.ServiceComponentData.FriendlyName, null, adSetup);
				}
				this.PublishStateChange(eventProducer, "Activated");
			}
			catch (Exception ex)
			{
				this.PublishStateChangeFailed(eventProducer, "ActivationFailed", ex);
				throw;
			}
		}

		public void Deactivate(IEventProducer eventProducer)
		{
			if (this.IServiceComponent != null)
				this.Stop(eventProducer);

			this.PublishStateChange(eventProducer, "Deactivating");

			try
			{
				if (this.WorkerDomain != null)
				{
					AppDomain.Unload(this.WorkerDomain);
					this.WorkerDomain = null;
				}

				this.PublishStateChange(eventProducer, "Deactivated");
			}
			catch (Exception ex)
			{
				this.PublishStateChangeFailed(eventProducer, "DeactivationFailed", ex);
				throw;
			}
		}

		public void Update(IEventProducer eventProducer, ServiceComponent serviceComponent)
		{
			this.Stop(eventProducer);
			this.ServiceComponentData = serviceComponent;
			this.Start(eventProducer);
		}

		private void PublishStateChange(IEventProducer eventProducer, string state)
		{
			eventProducer.Publish(new ServiceHostComponentStateChange() { Environment = this.Environment, Process = this.Process, ServiceComponent = this.ServiceComponentData.FriendlyName, State = state });
		}

		private void PublishStateChangeFailed(IEventProducer eventProducer, string state, Exception ex)
		{
			eventProducer.Publish(new ServiceHostComponentStateChangeFailed() { Environment = this.Environment, Process = this.Process, ServiceComponent = this.ServiceComponentData.FriendlyName, State = state, Exception = ex });
		}
	}
}
