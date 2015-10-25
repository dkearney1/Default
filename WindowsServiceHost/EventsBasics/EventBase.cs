using System;

namespace DKK.Events
{
	[Serializable]
	public abstract class EventBase : IEvent
	{
		protected abstract string routingKeyFormat { get; }
		protected abstract string routingKeyExplanation { get; }

		public EventBase()
		{
			this.Created = DateTimeOffset.Now;
			this.Machine = Environment.MachineName;
		}

		public DateTimeOffset Created { get; set; }
		public string Machine { get; set; }
		public string Process { get; set; }
		public string UserName { get; set; }
		public abstract string RoutingKey { get; }

		public string RoutingKeyExplanation => this.routingKeyExplanation;
	}
}
