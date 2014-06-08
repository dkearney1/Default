using DKK.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ServiceHostCommands
{
	public sealed class ActivateComponent : CommandBase
	{
		public Guid ServiceComponentId { get; private set; }

		public ActivateComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(ActivateComponentAck), typeof(ActivateComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
