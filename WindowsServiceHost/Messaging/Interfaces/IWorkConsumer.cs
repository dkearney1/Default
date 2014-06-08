using DKK.Work;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Messaging
{
	public interface IWorkConsumer : IChannel
	{
		void RegisterWorkHandler(string workQueueName, Action<IBasicProperties, IWork> action);
		bool UnregisterWorkHandler(string workQueueName);
	}
}
