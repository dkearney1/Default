using DKK.Commands;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Messaging
{
	public interface ICommandConsumer : IChannel
	{
		PublicationAddress PublicationAddress { get; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		void RegisterCommandHandler<T>(Func<IBasicProperties, ICommand, ICommand> action) where T : ICommand;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		bool UnregisterCommandHandler<T>() where T : ICommand;
	}
}
