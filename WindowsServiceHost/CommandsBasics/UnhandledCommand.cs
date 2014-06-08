using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Commands
{
	public sealed class UnhandledCommand : CommandBase
	{
		public UnhandledCommand()
			: base()
		{
		}

		public UnhandledCommand(ICommand src)
			: base(src)
		{
		}
	}
}
