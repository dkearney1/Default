using System;
using System.Globalization;
using DKK.Events;

namespace DKK.ServiceHostEvents
{
	[Serializable]
	public sealed class ServiceHostStateChange : EventBase
	{
		protected override string routingKeyFormat => @"ServiceHost.StateChange.{0}.{1}.{2}";

		protected override string routingKeyExplanation => @"ServiceHost.StateChange.Machine.Environment.State";

		public string Environment { get; set; }
		public string State { get; set; }

		public ServiceHostStateChange()
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
