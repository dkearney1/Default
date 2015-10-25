using DKK.Work;
using RabbitMQ.Client;

namespace DKK.Messaging
{
	public interface IWorkProducer : IChannel
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		void Publish(IWork work, string routingKey = null, IBasicProperties properties = null);
	}
}
