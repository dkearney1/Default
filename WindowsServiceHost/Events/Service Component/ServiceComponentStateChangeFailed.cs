using System;
using System.Globalization;
using DKK.Events;

namespace DKK.ServiceHostEvents
{
	[Serializable]
	public sealed class ServiceComponentStateChangeFailed : EventBase
	{
		protected override string routingKeyFormat => @"ServiceComponent.StateChangeFailed.{0}.{1}.{2}";

		protected override string routingKeyExplanation => @"ServiceComponent.StateChangeFailed.Machine.ServiceComponent.Status";

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
