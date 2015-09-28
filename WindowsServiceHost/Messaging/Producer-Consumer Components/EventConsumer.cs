using DKK.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DKK.Messaging
{
	public sealed class EventConsumer : ChannelBase, IEventConsumer
	{
		private sealed class SubscriptionInfo
		{
			public string RoutingKey { get; set; }
			public Action<IBasicProperties, IEvent> Handler { get; set; }
		}

		private ExchangeSettings EventExchange { get; set; }
		private QueueSettings PrivateQueue { get; set; }
		private ConcurrentDictionary<Type, SubscriptionInfo> RegisteredHandlers { get; set; }
		private ConcurrentQueue<Action> QueueBindingOps { get; set; }
		private CancellationTokenSource CancellationTokenSource { get; set; }
		private Task ConsumeTask { get; set; }

		public PublicationAddress PublicationAddress
		{
			get { return new PublicationAddress(this.EventExchange.ExchangeType, this.EventExchange.Name, this.PrivateQueue.Name); }
		}

		public EventConsumer(IConnection connection)
			: base(connection)
		{
			this.EventExchange = Constants.EventExchangeSettings;
			this.Channel.ExchangeDeclare(this.EventExchange.Name, this.EventExchange.ExchangeType, this.EventExchange.Durable, this.EventExchange.AutoDelete, this.EventExchange.Arguments);

			this.PrivateQueue = Constants.PrivateQueueSettings;
			this.Channel.QueueDeclare(this.PrivateQueue.Name, this.PrivateQueue.Durable, this.PrivateQueue.Exclusive, this.PrivateQueue.AutoDelete, this.PrivateQueue.Arguments);

			this.RegisteredHandlers = new ConcurrentDictionary<Type, SubscriptionInfo>();
			this.QueueBindingOps = new ConcurrentQueue<Action>();

			this.CancellationTokenSource = new CancellationTokenSource();

			this.ConsumeTask = Task.Run(() => this.Consume(this.CancellationTokenSource.Token));
		}

		public void RegisterEventHandler<T>(string routingKey, Action<IBasicProperties, IEvent> action)
			where T : IEvent
		{
            var t = typeof(T);

			if (this.RegisteredHandlers.ContainsKey(t))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Event Handler already registered for Type {0}", typeof(T).Name));

			if (this.RegisteredHandlers.Any(rh => rh.Value.RoutingKey == routingKey))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Event Handler already registered for Routing Key {0}", routingKey));

			this.RegisteredHandlers[t] = new SubscriptionInfo()
			{
				RoutingKey = routingKey,
				Handler = (props, evnt) => { action(props, evnt); }
			};

			this.QueueBindingOps.Enqueue(() => this.Channel.QueueBind(this.PrivateQueue.Name, this.EventExchange.Name, routingKey));
		}

		public bool UnregisterEventHandler<T>()
			where T : IEvent
		{
            var t = typeof(T);
			SubscriptionInfo si = null;

			if (this.RegisteredHandlers.TryRemove(t, out si))
			{
				this.QueueBindingOps.Enqueue(() => this.Channel.QueueUnbind(this.PrivateQueue.Name, this.EventExchange.Name, si.RoutingKey, null));
				return true;
			}

			return false;
		}

		public void Close()
		{
			this.Dispose();
		}

		#region IDisposable Members
		private bool disposed;

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.ConsumeTask != null)
					{
						this.CancellationTokenSource.Cancel();
						this.ConsumeTask.Wait();
						this.ConsumeTask = null;
						this.CancellationTokenSource = null;

						if (!this.PrivateQueue.AutoDelete)
						{
							foreach (var si in this.RegisteredHandlers)
							{
								this.Channel.QueueUnbind(this.PrivateQueue.Name, this.EventExchange.Name, si.Value.RoutingKey, null);
							}
						}
					}
				}

				base.Dispose(disposing);

				this.disposed = true;
			}
		}
		#endregion

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void Consume(CancellationToken ct)
		{
            var autoAck = false;

			using (Subscription subscription = new Subscription(this.Channel, this.PrivateQueue.Name, autoAck))
			{
                var subscriptionTimeout = TimeSpan.FromMilliseconds(100d).Milliseconds;

				while (!ct.IsCancellationRequested)
				{
					while (!this.QueueBindingOps.IsEmpty)
					{
						Action action = null;
						if (this.QueueBindingOps.TryDequeue(out action))
							action();
					}

					BasicDeliverEventArgs eventArgs = null;
					subscription.Next(subscriptionTimeout, out eventArgs);

					if (eventArgs != null && !ct.IsCancellationRequested)
					{
                        var evnt = EventDeserializer.Deserialize(eventArgs.Body) as IEvent;

                        var t = evnt.GetType();

						try
						{
							if (this.RegisteredHandlers.ContainsKey(t))
								this.RegisteredHandlers[t].Handler(eventArgs.BasicProperties, evnt);
						}
						catch
						{
							// eat any and all exceptions
							// consider accepting an exception handler Action<>()
							// and calling it
						}
						finally
						{
							subscription.Ack(eventArgs);
						}
					}
				}
			}
		}
	}
}
