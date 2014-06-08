using DKK.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ack")]
	public sealed class ActivateComponentAck : CommandBase
	{
		public ActivateComponentAck()
		{
		}

		public ActivateComponentAck(ICommand source)
			: base(source)
		{
		}
	}
}
