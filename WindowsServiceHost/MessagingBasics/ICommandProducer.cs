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
		ICommand Publish(ICommand evnt, PublicationAddress receiver);
		ICommand Publish(ICommand evnt, PublicationAddress receiver, TimeSpan timeout);
	}
}
