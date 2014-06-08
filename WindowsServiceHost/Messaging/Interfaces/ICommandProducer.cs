using DKK.Commands;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Messaging
{
	public interface ICommandProducer : IChannel
	{
		string QueueName { get; }
		ICommand Publish(ICommand command, PublicationAddress receiver, TimeSpan? timeout);
	}
}
