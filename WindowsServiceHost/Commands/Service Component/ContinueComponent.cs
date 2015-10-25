using System;
using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	public sealed class ContinueComponent : CommandBase
	{
		public Guid ServiceComponentId { get; }

		public ContinueComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(ContinueComponentAck), typeof(ContinueComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
