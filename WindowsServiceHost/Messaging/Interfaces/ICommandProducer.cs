using System;
using DKK.Commands;
using RabbitMQ.Client;

namespace DKK.Messaging
{
	public interface ICommandProducer : IChannel
	{
		string QueueName { get; }
		ICommand Publish(ICommand command, PublicationAddress receiver, TimeSpan? timeout);
	}
}
