using System;
using DKK.Events;
using RabbitMQ.Client;

namespace DKK.Messaging
{
	public interface IEventConsumer : IChannel
	{
		PublicationAddress PublicationAddress { get; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		void RegisterEventHandler<T>(string routingKey, Action<IBasicProperties, IEvent> action) where T : IEvent;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		bool UnregisterEventHandler<T>() where T : IEvent;
	}
}
