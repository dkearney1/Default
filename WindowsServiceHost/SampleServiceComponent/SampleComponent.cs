using DKK.Events;
using DKK.Messaging;
using DKK.ServiceHostEvents;
using DKK.WindowsServiceComponentInterface;
using DKK.Work;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DKK.SampleServiceComponent
{
	[Serializable]
	public sealed class SampleComponent : MarshalByRefObject, IWindowsServiceComponent, IDisposable
	{
		private enum InternalStateEnum
		{
			Idle,
			Paused,
			Busy,
		}

		private bool Paused;
		private string Process { get; set; }
		private string ServiceComponent { get; set; }
		private TimeSpan IdleMessageRate { get; set; }
		private string WorkQueue { get; set; }
		private Task Task { get; set; }
		private CancellationTokenSource CancellationTokenSource { get; set; }
		private MessageBrokerConnection MessageBrokerConnection { get; set; }
		private IEventProducer EventProducer { get; set; }
		private InternalStateEnum InternalState { get; set; }
		private IEvent LastEvent { get; set; }
		private DateTime LastPublish { get; set; }
		private object PublishLock;

		public string ConfigURL { get; set; }
		public string Configuration { get; set; }
		public IEnumerable<KeyValuePair<string, string>> Environment { get; set; }

		public SampleComponent()
		{
			this.Process = Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().CodeBase);
			this.ServiceComponent = this.GetType().Name;
			this.PublishLock = new object();
		}

		public void Start()
		{
			this.PublishEvent("Starting");
			try
			{
				this.IdleMessageRate = TimeSpan.FromSeconds(5d);
				if (this.Configuration != null)
				{
					try
					{
                        var param = JsonConvert.DeserializeObject<SampleComponentParams>(this.Configuration, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
						this.IdleMessageRate = TimeSpan.FromSeconds(param.IdleMessageRate);
						this.WorkQueue = param.WorkQueue;
					}
					catch
					{ /* eating the exception */ }
				}

				this.CancellationTokenSource = new CancellationTokenSource();
				this.Task = Task.Run(() => this.InternalTask());
			}
			catch (Exception ex)
			{
				this.PublishEventFailed("StartFailed", ex);
				throw;
			}
		}

		public void Stop()
		{
			try
			{
				if (this.Task != null && this.CancellationTokenSource != null)
				{
					this.CancellationTokenSource.Cancel();
					this.Task.Wait();
					this.Task = null;
					this.CancellationTokenSource = null;
				}
				this.PublishEvent("Stopped");
			}
			catch (Exception ex)
			{
				this.PublishEventFailed("StopFailed", ex);
				throw;
			}
		}

		public void Pause()
		{
			this.PublishEvent("Pausing");
			try
			{
				this.Paused = true;
			}
			catch (Exception ex)
			{
				this.PublishEventFailed("PauseFailed", ex);
				throw;
			}
		}

		public void Continue()
		{
			this.PublishEvent("Continuing");
			try
			{
				this.Paused = false;
			}
			catch (Exception ex)
			{
				this.PublishEventFailed("ContinueFailed", ex);
				throw;
			}
		}

		private void PublishEvent(string status, string substatus = null)
		{
            var evnt = new ServiceComponentStatus() { Process = this.Process, ServiceComponent = this.ServiceComponent, Status = status, SubStatus = substatus };
			this.PublishEvent(evnt);
		}

		private void PublishEventFailed(string status, Exception ex)
		{
            var evnt = new ServiceComponentStateChangeFailed() { Process = this.Process, ServiceComponent = this.ServiceComponent, Status = status, Exception = ex };
			this.PublishEvent(evnt);
		}

		private void PublishEvent(IEvent evnt)
		{
			// don't let simultaneous publishes happen
			lock (this.PublishLock)
			{
				if (this.EventProducer == null)
				{
					if (this.MessageBrokerConnection == null)
						this.MessageBrokerConnection = new MessageBrokerConnection(this.Environment);
					this.EventProducer = new EventProducer(this.MessageBrokerConnection.Connection);
				}

				this.EventProducer.Publish(evnt);
				this.LastEvent = evnt;
				this.LastPublish = DateTime.Now;
			}
		}

		private void InternalTask()
		{
			this.PublishEvent("Started");
            var sleepSpan = TimeSpan.FromMilliseconds(100d);
            var routingKey = this.WorkQueue;

            var heartbeat = Task.Run(() =>
			{
				CancellationToken hbct = this.CancellationTokenSource.Token;

				while (!hbct.IsCancellationRequested)
				{
					Thread.Sleep(sleepSpan);
					if (DateTime.Now > (this.LastPublish + this.IdleMessageRate) && this.LastEvent != null)
						this.PublishEvent(this.LastEvent);
				}
			});

            var ct = this.CancellationTokenSource.Token;

			using (var wc = new WorkConsumer(this.MessageBrokerConnection.Connection))
			{
				wc.RegisterWorkHandler(routingKey, this.WorkItemHandler);

				while (!ct.IsCancellationRequested)
				{
					if (this.Paused)
					{
						wc.UnregisterWorkHandler(routingKey);

						this.InternalState = InternalStateEnum.Paused;
						this.PublishEvent("Paused");

						while (this.Paused && !ct.IsCancellationRequested)
							Thread.Sleep(sleepSpan);

						if (!ct.IsCancellationRequested)
							wc.RegisterWorkHandler(routingKey, this.WorkItemHandler);

						this.InternalState = InternalStateEnum.Idle;
						this.PublishEvent("Idle");
					}

					if (!ct.IsCancellationRequested)
						Thread.Sleep(sleepSpan);
				}
			}

			this.PublishEvent("Stopping");

			heartbeat.Wait();
		}

		private void WorkItemHandler(IBasicProperties props, IWork iWork)
		{
            var work = iWork as SampleWorkItem;
			if (work == null)
				throw new Exception(string.Format("Don't know how to work on '{0}' items", iWork.GetType().FullName));

			this.InternalState = InternalStateEnum.Busy;

            var currentStatus = new ServiceComponentStatus() { Process = this.Process, ServiceComponent = this.GetType().Name, Status = "Working", SubStatus = "Starting" };
			this.PublishEvent(currentStatus);

            var workItemType = work.ItemType;
            var workItemId = work.ItemId;
            var workDelay = work.WorkDelay;

			currentStatus.SubStatus = "Executing";
			this.PublishEvent(currentStatus);

			Thread.Sleep(workDelay);

			currentStatus.SubStatus = "Completing";
			this.PublishEvent(currentStatus);

			this.InternalState = InternalStateEnum.Idle;
			this.PublishEvent("Idle");
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
					this.MessageBrokerConnection.Dispose();
					this.MessageBrokerConnection = null;
				}
			}

			disposed = true;
		}
		#endregion
	}
}
