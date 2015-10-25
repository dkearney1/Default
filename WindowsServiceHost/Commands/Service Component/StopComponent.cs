using System;
using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	public sealed class StopComponent : CommandBase
	{
		public Guid ServiceComponentId { get; }

		public StopComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(StopComponentAck), typeof(StopComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
