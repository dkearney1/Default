using DKK.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Messaging
{
	public interface IEventProducer : IChannel
	{
		void Publish(IEvent evnt);
		void Publish(string routingKey, IEvent evnt);
		void Publish(string routingKey, IBasicProperties properties, IEvent evnt);
	}
}
