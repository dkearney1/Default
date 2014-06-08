using DKK.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ServiceHostCommands
{
	public sealed class PauseComponent : CommandBase
	{
		public Guid ServiceComponentId { get; private set; }

		public PauseComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(PauseComponentAck), typeof(PauseComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
