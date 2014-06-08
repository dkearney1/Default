using DKK.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ServiceHostCommands
{
	public sealed class StopComponent : CommandBase
	{
		public Guid ServiceComponentId { get; private set; }

		public StopComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(StopComponentAck), typeof(StopComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
