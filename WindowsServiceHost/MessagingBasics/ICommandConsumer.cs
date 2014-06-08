using DKK.Commands;
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
		void RegisterCommandHandler<T>(Func<IBasicProperties, ICommand, ICommand> action) where T : ICommand;
		bool UnregisterCommandHandler<T>() where T : ICommand;
	}
}
