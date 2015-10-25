using System;
using System.Globalization;
using DKK.Events;

namespace DKK.ServiceHostEvents
{
	[Serializable]
	public sealed class ServiceHostStateChangeFailed : EventBase
	{
		protected override string routingKeyFormat => @"ServiceHost.StateChangeFailed.{0}.{1}.{2}";

		protected override string routingKeyExplanation => @"ServiceHost.StateChangeFailed.Machine.Environment.State";

		public string Environment { get; set; }
		public string State { get; set; }
		public Exception Exception { get; set; }

		public ServiceHostStateChangeFailed()
			: base()
		{
		}

		public override string RoutingKey
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.State))
					throw new InvalidOperationException("'State' has not been set");

				return string.Format(CultureInfo.InvariantCulture, this.routingKeyFormat, this.Machine, this.Environment, this.State);
			}
		}
	}
}
