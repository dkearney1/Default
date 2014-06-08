using DKK.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Messaging
{
	public sealed class EventProducer : ChannelBase, IEventProducer
	{
		public EventProducer(IConnection connection)
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
		public void Publish(IEvent evnt, string routingKey = null, IBasicProperties properties = null)
		{
			if (evnt == null)
				throw new ArgumentNullException("evnt");

			if (string.IsNullOrWhiteSpace(routingKey))
				routingKey = evnt.RoutingKey;

			if (properties == null)
			{
				properties = this.Channel.CreateBasicProperties();

				//props.AppId = string.Empty;
				//props.ClusterId = string.Empty;
				//props.ContentEncoding = string.Empty;
				//props.ContentType = "application/json"; // Set by the Message Serializer
				//props.CorrelationId = string.Empty;
				properties.DeliveryMode = Constants.NonPersistent;
				//props.Expiration = string.Empty;
				//props.MessageId = string.Empty;
				//props.Priority = 0;
				//props.ReplyTo = string.Empty;
				//props.Type = msg.GetType().Name; // Set by the Message Serializer, use this field to store the type of object encoded in the Body. Allows for a factory deserializer to easily determine what to return
				//props.UserId = string.Empty;
			}

			if (!this.Channel.IsOpen)
				throw new InvalidOperationException("Channel is not open");

			byte[] bytes = EventSerializer.Serialize(evnt);
			properties.ContentEncoding = EventSerializer.ContentEncoding;
			properties.ContentType = EventSerializer.ContentType;
			properties.Type = evnt.GetType().FullName;

			ExchangeSettings eventsExchange = Constants.EventExchangeSettings;
			this.Channel.ExchangeDeclare(eventsExchange.Name, eventsExchange.ExchangeType, eventsExchange.Durable, eventsExchange.AutoDelete, eventsExchange.Arguments);
			this.Channel.BasicPublish(eventsExchange.Name, routingKey, properties, bytes);
		}
	}
}
