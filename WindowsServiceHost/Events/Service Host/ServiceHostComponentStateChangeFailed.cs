using System;
using System.Globalization;
using DKK.Events;

namespace DKK.ServiceHostEvents
{
	[Serializable]
	public sealed class ServiceHostComponentStateChangeFailed : EventBase
	{
		protected override string routingKeyFormat => @"ServiceHostComponent.StateChangeFailed.{0}.{1}.{2}.{3}";

		protected override string routingKeyExplanation => @"ServiceHostComponent.StateChangeFailed.Machine.Environment.Component.State";

		public string Environment { get; set; }
		public string ServiceComponent { get; set; }
		public string State { get; set; }
		public Exception Exception { get; set; }

		public ServiceHostComponentStateChangeFailed()
			: base()
		{
		}

		public override string RoutingKey
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.State))
					throw new InvalidOperationException("'State' has not been set");

				return string.Format(CultureInfo.InvariantCulture, this.routingKeyFormat, this.Machine, this.Environment, this.ServiceComponent, this.State);
			}
		}
	}
}
