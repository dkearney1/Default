using DKK.Work;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Messaging
{
	public interface IWorkProducer : IChannel
	{
		void Publish(string routingKey, IWork work);
		void Publish(string routingKey, IBasicProperties properties, IWork work);
	}
}
