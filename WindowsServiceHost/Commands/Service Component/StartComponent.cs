using System;
using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	public sealed class StartComponent : CommandBase
	{
		public Guid ServiceComponentId { get; }

		public StartComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(StartComponentAck), typeof(StartComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
