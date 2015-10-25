using System;

namespace DKK.Events
{
	[Serializable]
	public class QueueChangeEventBase : EventBase, IQueueChangeEvent
	{
		protected override string routingKeyFormat => @"Queue.{0}.{1}.{2}";

		protected override string routingKeyExplanation => @"Queue.From.To.Type";

		public string FromQueue { get; set; }
		public string ToQueue { get; set; }
		public string Type { get; set; }

		public QueueChangeEventBase()
		{
		}

		public override string RoutingKey => string.Format(this.routingKeyFormat, this.FromQueue, this.ToQueue, this.Type);
	}
}
