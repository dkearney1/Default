using DKK.Commands;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace DKK.Messaging
{
	public sealed class CommandConsumer : ChannelBase, ICommandConsumer
	{
		private ExchangeSettings CmdExchange { get; set; }
		private QueueSettings PrivateQueue { get; set; }
		private ConcurrentDictionary<Type, Func<IBasicProperties, ICommand, ICommand>> RegisteredHandlers { get; set; }
		private Func<IBasicProperties, ICommand, ICommand> UnhandledCommand { get; set; }
		private CancellationTokenSource CancellationTokenSource { get; set; }
		private Task ConsumeTask { get; set; }

		public PublicationAddress PublicationAddress
		{
			get { return new PublicationAddress(this.CmdExchange.ExchangeType, this.CmdExchange.Name, this.PrivateQueue.Name); }
		}

		public CommandConsumer(IConnection connection)
			: base(connection)
		{
			this.CmdExchange = Constants.CmdExchangeSettings;
			this.Channel.ExchangeDeclare(this.CmdExchange.Name, this.CmdExchange.ExchangeType, this.CmdExchange.Durable, this.CmdExchange.AutoDelete, this.CmdExchange.Arguments);

			this.PrivateQueue = Constants.PrivateQueueSettings;
			this.Channel.QueueDeclare(this.PrivateQueue.Name, this.PrivateQueue.Durable, this.PrivateQueue.Exclusive, this.PrivateQueue.AutoDelete, this.PrivateQueue.Arguments);
			this.Channel.QueueBind(this.PrivateQueue.Name, this.CmdExchange.Name, this.PrivateQueue.Name);

			this.RegisteredHandlers = new ConcurrentDictionary<Type, Func<IBasicProperties, ICommand, ICommand>>();
			this.UnhandledCommand = ((reqProps, cmd) =>
			{
				return new UnhandledCommand(cmd);
			});

			this.CancellationTokenSource = new CancellationTokenSource();

			this.ConsumeTask = Task.Run(() => this.Consume(this.CancellationTokenSource.Token));
		}

		public void RegisterCommandHandler<T>(Func<IBasicProperties, ICommand, ICommand> action)
			where T : ICommand
		{
            var t = typeof(T);

			if (this.RegisteredHandlers.ContainsKey(t))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Command Handler already registered for {0}", typeof(T).Name));

			this.RegisteredHandlers[t] = (props, cmd) =>
			{
				return action(props, cmd);
			};
		}

		public bool UnregisterCommandHandler<T>()
			where T : ICommand
		{
            var t = typeof(T);
			Func<IBasicProperties, ICommand, ICommand> action;

			return this.RegisteredHandlers.TryRemove(t, out action);
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
							this.Channel.QueueUnbind(this.PrivateQueue.Name, this.CmdExchange.Name, this.PrivateQueue.Name, null);
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

			using (var subscription = new Subscription(this.Channel, this.PrivateQueue.Name, autoAck))
			{
                var subscriptionTimeout = TimeSpan.FromMilliseconds(100d).Milliseconds;

				while (!ct.IsCancellationRequested)
				{
					BasicDeliverEventArgs eventArgs = null;
					subscription.Next(subscriptionTimeout, out eventArgs);

					if (eventArgs != null && !ct.IsCancellationRequested)
					{
                        var cmd = CommandDeserializer.Deserialize(eventArgs.Body) as ICommand;

                        var t = cmd.GetType();
						Func<IBasicProperties, ICommand, ICommand> action;
						ICommand reply = null;

						try
						{
							if (this.RegisteredHandlers.TryGetValue(t, out action))
								reply = action(eventArgs.BasicProperties, cmd);
							else
								reply = this.UnhandledCommand(eventArgs.BasicProperties, cmd);
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
							if (reply != null)
								SendReply(eventArgs, reply);
						}
					}
				}
			}
		}

		private void SendReply(BasicDeliverEventArgs cmdEventArgs, ICommand reply)
		{
			if (reply.CorrelationId.HasValue)
			{
                var props = this.Channel.CreateBasicProperties();

				//props.AppId;
				//props.ClusterId;
				props.CorrelationId = reply.CorrelationId.ToString();
				props.DeliveryMode = Constants.Persistent;
                //props.Expiration;
                //props.Headers;
                //props.MessageId;
                //props.Priority;
                //props.ProtocolClassId;
                //props.ProtocolClassName;
                //props.ReplyTo;
                //props.ReplyToAddress = new PublicationAddress(cmdExchange.Type, cmdExchange.Name, privateQueue.Name);
                //props.Timestamp;
                //props.UserId;

                var bytes = CommandSerializer.Serialize(reply);
				props.ContentEncoding = CommandSerializer.ContentEncoding;
				props.ContentType = CommandSerializer.ContentType;
				props.Type = reply.GetType().FullName;

				if (cmdEventArgs.BasicProperties.ReplyToAddress != null)
					this.Channel.BasicPublish(cmdEventArgs.BasicProperties.ReplyToAddress, props, bytes);
				else if (!string.IsNullOrWhiteSpace(cmdEventArgs.BasicProperties.ReplyTo))
					this.Channel.BasicPublish(this.CmdExchange.Name, cmdEventArgs.BasicProperties.ReplyTo, props, bytes);
				else
					this.Channel.BasicPublish(this.CmdExchange.Name, reply.GetType().Name, props, bytes);
			}
		}
	}
}
