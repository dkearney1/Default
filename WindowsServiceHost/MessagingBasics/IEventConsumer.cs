using DKK.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Messaging
{
	public interface IEventConsumer : IChannel
	{
		PublicationAddress PublicationAddress { get; }
		void RegisterEventHandler<T>(string routingKey, Action<IBasicProperties, IEvent> action) where T : IEvent;
		bool UnregisterEventHandler<T>() where T : IEvent;
	}
}
