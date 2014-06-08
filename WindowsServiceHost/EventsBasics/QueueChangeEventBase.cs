using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Events
{
	[Serializable]
	public class QueueChangeEventBase : EventBase, IQueueChangeEvent
	{
		protected override string routingKeyFormat
		{
			get { return @"Queue.{0}.{1}.{2}"; }
		}

		protected override string routingKeyExplanation
		{
			get { return @"Queue.From.To.Type"; }
		}

		public string FromQueue { get; set; }
		public string ToQueue { get; set; }
		public string Type { get; set; }

		public QueueChangeEventBase()
		{
		}

		public override string RoutingKey
		{
			get { return string.Format(this.routingKeyFormat, this.FromQueue, this.ToQueue, this.Type); }
		}
	}
}
