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
	public sealed class ReloadConfigurationAck : CommandBase
	{
		public ReloadConfigurationAck()
		{
		}

		public ReloadConfigurationAck(ICommand source)
			: base(source)
		{
		}
	}
}
