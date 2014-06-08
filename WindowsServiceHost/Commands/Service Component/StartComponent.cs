using DKK.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ServiceHostCommands
{
	public sealed class StartComponent : CommandBase
	{
		public Guid ServiceComponentId { get; private set; }

		public StartComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(StartComponentAck), typeof(StartComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
