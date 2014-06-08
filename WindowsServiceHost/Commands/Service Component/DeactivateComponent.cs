using DKK.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ServiceHostCommands
{
	public sealed class DeactivateComponent : CommandBase
	{
		public Guid ServiceComponentId { get; private set; }

		public DeactivateComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(DeactivateComponentAck), typeof(DeactivateComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
