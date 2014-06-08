using DKK.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ServiceHostEvents
{
	[Serializable]
	public sealed class ServiceComponentStateChangeFailed : EventBase
	{
		protected override string routingKeyFormat
		{
			get { return @"ServiceComponent.StateChangeFailed.{0}.{1}.{2}"; }
		}

		protected override string routingKeyExplanation
		{
			get { return @"ServiceComponent.StateChangeFailed.Machine.ServiceComponent.Status"; }
		}

		public string ServiceComponent { get; set; }
		public Exception Exception { get; set; }
		public string Status { get; set; }

		public ServiceComponentStateChangeFailed()
			: base()
		{
		}

		public override string RoutingKey
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.Status))
					throw new InvalidOperationException("'Status' has not been set");
				return string.Format(CultureInfo.InvariantCulture, this.routingKeyFormat, this.Machine, this.ServiceComponent, this.Status);
			}
		}
	}
}
