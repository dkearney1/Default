using System;
using DKK.Commands;

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
