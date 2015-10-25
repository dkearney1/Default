using System;
using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	public sealed class ActivateComponent : CommandBase
	{
		public Guid ServiceComponentId { get; }

		public ActivateComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(ActivateComponentAck), typeof(ActivateComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
