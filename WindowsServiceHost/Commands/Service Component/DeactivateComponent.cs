using System;
using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	public sealed class DeactivateComponent : CommandBase
	{
		public Guid ServiceComponentId { get; }

		public DeactivateComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(DeactivateComponentAck), typeof(DeactivateComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
