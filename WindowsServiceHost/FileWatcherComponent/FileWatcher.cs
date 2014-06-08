using DKK.Events;
using DKK.Messaging;
using DKK.ServiceHostEvents;
using DKK.WindowsServiceComponentInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DKK.FileWatcherComponent
{
	[Serializable]
	public sealed class FileWatcher : MarshalByRefObject, IWindowsServiceComponent, IDisposable
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

		public FileWatcher()
		{
			this.Process = Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().CodeBase);
			this.ServiceComponent = this.GetType().Name;
			this.PublishLock = new object();
		}

		public void Start()
		{
			this.PublishSCEvent("Starting");
			try
			{
				this.IdleMessageRate = TimeSpan.FromSeconds(5d);
				if (this.Configuration != null)
				{
					try
					{
						FileWatcherParams param = JsonConvert.DeserializeObject<FileWatcherParams>(this.Configuration, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
					}
					catch
					{ /* eating the exception */ }
				}

				this.CancellationTokenSource = new CancellationTokenSource();
				this.Task = Task.Run(() => this.InternalTask());
			}
			catch (Exception ex)
			{
				this.PublishSCEventFailed("StartFailed", ex);
				throw;
			}
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}

		public void Pause()
		{
			throw new NotImplementedException();
		}

		public void Continue()
		{
			throw new NotImplementedException();
		}

		private void PublishSCEvent(string status, string substatus = null)
		{
			IEvent evnt = new ServiceComponentStatus() { Process = this.Process, ServiceComponent = this.ServiceComponent, Status = status, SubStatus = substatus };
			this.PublishSCEvent(evnt);
		}

		private void PublishSCEventFailed(string status, Exception ex)
		{
			IEvent evnt = new ServiceComponentStateChangeFailed() { Process = this.Process, ServiceComponent = this.ServiceComponent, Status = status, Exception = ex };
			this.PublishSCEvent(evnt);
		}

		private void PublishSCEvent(IEvent evnt)
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

		#region MarshalByRefObject
		public override object InitializeLifetimeService()
		{
			ILease lease = (ILease)base.InitializeLifetimeService();
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
