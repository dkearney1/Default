using System;
using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	public sealed class ReloadFiles : CommandBase
	{
		public ReloadFiles()
		{
			this._responseTypes.AddRange(new Type[] { typeof(ReloadFilesAck), typeof(ReloadFilesNack) });
		}
	}
}
