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
	public sealed class ServiceHostStateChangeFailed : EventBase
	{
		protected override string routingKeyFormat
		{
			get { return @"ServiceHost.StateChangeFailed.{0}.{1}.{2}"; }
		}

		protected override string routingKeyExplanation
		{
			get { return @"ServiceHost.StateChangeFailed.Machine.Environment.State"; }
		}

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
