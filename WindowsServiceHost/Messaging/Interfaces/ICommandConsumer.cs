using System;
using DKK.Commands;
using RabbitMQ.Client;

namespace DKK.Messaging
{
	public interface ICommandConsumer : IChannel
	{
		PublicationAddress PublicationAddress { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		void RegisterCommandHandler<T>(Func<IBasicProperties, ICommand, ICommand> action)
			where T : ICommand;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		bool UnregisterCommandHandler<T>()
			where T : ICommand;
	}
}
