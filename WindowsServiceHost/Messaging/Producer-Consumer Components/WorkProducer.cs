using DKK.Work;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Messaging
{
	public sealed class WorkProducer : ChannelBase, IWorkProducer
	{
		public WorkProducer(IConnection connection)
			: base(connection)
		{
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
				}

				base.Dispose(disposing);

				this.disposed = true;
			}
		}
		#endregion

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public void Publish(IWork work, string routingKey = null, IBasicProperties properties = null)
		{
			if (work == null)
				throw new ArgumentNullException("work");

			if (string.IsNullOrWhiteSpace(routingKey))
				throw new ArgumentNullException("routingKey");

			if (properties == null)
			{
				properties = this.Channel.CreateBasicProperties();

				//properties.AppId = string.Empty;
				//properties.ClusterId = string.Empty;
				//properties.ContentEncoding = string.Empty;
				//properties.ContentType = "application/json"; // Set by the Message Serializer
				//properties.CorrelationId = string.Empty;
				properties.DeliveryMode = Constants.Persistent;
				//properties.Expiration = string.Empty;
				//properties.MessageId = string.Empty;
				//properties.Priority = 0;
				//properties.ReplyTo = string.Empty;
				//properties.Type = msg.GetType().Name; // Set by the Message Serializer, use this field to store the type of object encoded in the Body. Allows for a factory deserializer to easily determine what to return
				//properties.UserId = string.Empty;
			}

			if (!this.Channel.IsOpen)
				throw new InvalidOperationException("Channel is not open");

			byte[] bytes = WorkSerializer.Serialize(work);
			properties.ContentEncoding = WorkSerializer.ContentEncoding;
			properties.ContentType = WorkSerializer.ContentType;
			properties.Type = work.GetType().FullName;

			ExchangeSettings WorkExchange = Constants.WorkExchangeSettings;
			this.Channel.ExchangeDeclare(WorkExchange.Name, WorkExchange.ExchangeType, WorkExchange.Durable, WorkExchange.AutoDelete, WorkExchange.Arguments);
			this.Channel.BasicPublish(WorkExchange.Name, routingKey, properties, bytes);
		}
	}
}
