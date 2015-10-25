using System;
using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	public sealed class PauseComponent : CommandBase
	{
		public Guid ServiceComponentId { get; }

		public PauseComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(PauseComponentAck), typeof(PauseComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
