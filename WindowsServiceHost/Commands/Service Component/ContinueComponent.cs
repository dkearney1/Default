using DKK.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ServiceHostCommands
{
	public sealed class ContinueComponent : CommandBase
	{
		public Guid ServiceComponentId { get; private set; }

		public ContinueComponent(Guid serviceComponentId)
		{
			this._responseTypes.AddRange(new Type[] { typeof(ContinueComponentAck), typeof(ContinueComponentNack) });
			this.ServiceComponentId = serviceComponentId;
		}
	}
}
