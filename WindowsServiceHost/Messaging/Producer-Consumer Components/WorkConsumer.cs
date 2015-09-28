using DKK.Work;
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
	public sealed class WorkConsumer : ChannelBase, IWorkConsumer
	{
		private sealed class SubscriptionInfo
		{
			public QueueSettings WorkQueue { get; set; }
			public Action<IBasicProperties, IWork> Handler { get; set; }
		}

		private ExchangeSettings WorkExchange { get; set; }
		private ConcurrentDictionary<string, SubscriptionInfo> RegisteredHandlers { get; set; }
		private ConcurrentQueue<Action> QueueBindingOps { get; set; }
		private CancellationTokenSource CancellationTokenSource { get; set; }
		private Task ConsumeTask { get; set; }

		public WorkConsumer(IConnection connection)
			: base(connection)
		{
			this.WorkExchange = Constants.WorkExchangeSettings;
			this.Channel.ExchangeDeclare(this.WorkExchange.Name, this.WorkExchange.ExchangeType, this.WorkExchange.Durable, this.WorkExchange.AutoDelete, this.WorkExchange.Arguments);

			this.RegisteredHandlers = new ConcurrentDictionary<string, SubscriptionInfo>();
			this.QueueBindingOps = new ConcurrentQueue<Action>();

			this.CancellationTokenSource = new CancellationTokenSource();

			this.ConsumeTask = Task.Run(() => this.Consume(this.CancellationTokenSource.Token));
		}

		public void RegisterWorkHandler(string workQueueName, Action<IBasicProperties, IWork> action)
		{
			if (this.RegisteredHandlers.ContainsKey(workQueueName))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Work Handler already registered for {0}", workQueueName));

			var workQueue = Constants.WorkQueueSettings;
			workQueue.Name = workQueueName;

			// The queue has to be declared and bound first
			this.QueueBindingOps.Enqueue(() => this.Channel.QueueDeclare(workQueue.Name, workQueue.Durable, workQueue.Exclusive, workQueue.AutoDelete, workQueue.Arguments));
			this.QueueBindingOps.Enqueue(() => this.Channel.QueueBind(workQueue.Name, this.WorkExchange.Name, workQueue.Name));
			this.QueueBindingOps.Enqueue(() => this.Channel.BasicQos(0, 1, false));

			this.RegisteredHandlers[workQueueName] = new SubscriptionInfo()
			{
				WorkQueue = workQueue,
				Handler = (props, evnt) => { action(props, evnt); }
			};
		}

		public bool UnregisterWorkHandler(string workQueueName)
		{
			SubscriptionInfo si = null;

			if (this.RegisteredHandlers.TryRemove(workQueueName, out si))
			{
				this.QueueBindingOps.Enqueue(() => this.Channel.QueueUnbind(si.WorkQueue.Name, this.WorkExchange.Name, si.WorkQueue.Name, null));
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
					}

					this.RegisteredHandlers.Clear();
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

            var subscriptionTimeout = TimeSpan.FromMilliseconds(100d);

			while (!ct.IsCancellationRequested)
			{
				while (!this.QueueBindingOps.IsEmpty)
				{
					Action action = null;
					if (this.QueueBindingOps.TryDequeue(out action))
						action();
				}

                var workDone = false;

				foreach (var si in this.RegisteredHandlers.Values)
				{
                    var result = this.Channel.BasicGet(si.WorkQueue.Name, autoAck);

					if (result != null && !ct.IsCancellationRequested)
					{
                        var work = WorkDeserializer.Deserialize(result.Body) as IWork;

						try
						{
							si.Handler(result.BasicProperties, work);
							workDone = true;
							// Once work is found on any queue,
							// return to the top of the list of handlers,
							// which makes the order of "registration"
							// become the priority
							break;
						}
						catch (Exception /*ex*/)
						{
                            // eat any and all exceptions
                            // consider accepting an exception handler Action<>()
                            // and calling it

                            //this.Channel.BasicNack(result.DeliveryTag, false, true);
						}
						finally
						{
							// If an exception is thrown processing a work item,
							// it's not a good idea to automatically put it back
							// in the queue to be reprocessed. Well behaved work
							// will both send an event about the exception, and
							// persist the work in such a state that it can be
							// discovered and dealt with.
							this.Channel.BasicAck(result.DeliveryTag, false);
						}
					}
				}

				if (!workDone)
				{
					try
					{
						Task.Delay(subscriptionTimeout, ct).Wait();
					}
					catch(Exception /*ex*/)
					{
						// eat any and all exceptions
					}
				}
			}
		}
	}
}
