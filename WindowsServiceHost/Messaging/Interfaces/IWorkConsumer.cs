using System;
using DKK.Work;
using RabbitMQ.Client;

namespace DKK.Messaging
{
	public interface IWorkConsumer : IChannel
	{
		void RegisterWorkHandler(string workQueueName, Action<IBasicProperties, IWork> action);
		bool UnregisterWorkHandler(string workQueueName);
	}
}
