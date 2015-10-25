using System;
using System.Threading;
using System.Threading.Tasks;
using DKK.Commands;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace DKK.Messaging
{
	public sealed class CommandProducer : ChannelBase, ICommandProducer
	{
		private ExchangeSettings CmdExchange { get; }
		private QueueSettings PrivateQueue { get; }

		public string QueueName => (this.PrivateQueue == null ? null : this.PrivateQueue.Name);

		public CommandProducer(IConnection connection)
			: base(connection)
		{
			this.CmdExchange = Constants.CmdExchangeSettings;
			this.Channel.ExchangeDeclare(this.CmdExchange.Name, this.CmdExchange.ExchangeType, this.CmdExchange.Durable, this.CmdExchange.AutoDelete, this.CmdExchange.Arguments);

			this.PrivateQueue = Constants.PrivateQueueSettings;
			this.Channel.QueueDeclare(this.PrivateQueue.Name, this.PrivateQueue.Durable, this.PrivateQueue.Exclusive, this.PrivateQueue.AutoDelete, this.PrivateQueue.Arguments);
			this.Channel.QueueBind(this.PrivateQueue.Name, this.CmdExchange.Name, this.PrivateQueue.Name);
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
					if (!this.PrivateQueue.AutoDelete)
						this.Channel.QueueUnbind(this.PrivateQueue.Name, this.CmdExchange.Name, this.PrivateQueue.Name, null);
				}

				base.Dispose(disposing);

				this.disposed = true;
			}
		}
		#endregion

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public ICommand Publish(ICommand command, PublicationAddress receiver, TimeSpan? timeout = null)
		{
			if (!timeout.HasValue)
				timeout = TimeSpan.MaxValue;

			var props = this.Channel.CreateBasicProperties();

			//props.AppId;
			//props.ClusterId;
			//props.CorrelationId;
			props.DeliveryMode = Constants.Persistent;
			//props.Expiration;
			//props.Headers;
			//props.MessageId;
			//props.Priority;
			//props.ProtocolClassId;
			//props.ProtocolClassName;
			//props.ReplyTo;
			props.ReplyToAddress = new PublicationAddress(this.CmdExchange.ExchangeType, this.CmdExchange.Name, this.PrivateQueue.Name);
			//props.Timestamp;
			//props.UserId;

			var reqBytes = CommandSerializer.Serialize(command);
			props.ContentEncoding = CommandSerializer.ContentEncoding;
			props.ContentType = CommandSerializer.ContentType;
			props.Type = command.GetType().FullName;

			// at this point, we're ready to send

			// set up to receive the response before the send occurs
			// so there's no chance it'll be missed
			ICommand reply = null;

			using (CancellationTokenSource cts = new CancellationTokenSource())
			{
				var ct = cts.Token;

				Task replyTask = null;
				if (command.CorrelationId.HasValue)
				{
					props.CorrelationId = command.CorrelationId.ToString();

					replyTask = Task.Run(() =>
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
									reply = CommandDeserializer.Deserialize(eventArgs.Body) as ICommand;
									if (reply != null && reply.CorrelationId == command.CorrelationId)
									{
										subscription.Ack(eventArgs);
										break;
									}
									else
										subscription.Model.BasicNack(eventArgs.DeliveryTag, false, true);
								}
							}
						}
					});
				}

				// now the receiver is set up, publish the command
				this.Channel.BasicPublish(receiver, props, reqBytes);

				//Console.WriteLine("{0} Command sent ({1})", command.GetType().Name, command.CorrelationId);

				if (timeout < TimeSpan.MaxValue)
					cts.CancelAfter(timeout.Value);

				replyTask.Wait();

				return reply;
			}
		}
	}
}
