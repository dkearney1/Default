using DKK.Events;
using RabbitMQ.Client;

namespace DKK.Messaging
{
	public interface IEventProducer : IChannel
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "evnt")]
		void Publish(IEvent evnt, string routingKey = null, IBasicProperties properties = null);
	}
}
