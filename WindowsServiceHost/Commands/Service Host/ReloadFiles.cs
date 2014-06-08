using DKK.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
