using DKK.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ServiceHostCommands
{
	public sealed class ReloadConfiguration : CommandBase
	{
		public ReloadConfiguration()
		{
			this._responseTypes.AddRange(new Type[] { typeof(ReloadConfigurationAck), typeof(ReloadConfigurationNack) });
		}
	}
}
