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
	public sealed class ServiceHostComponentStateChange : EventBase
	{
		protected override string routingKeyFormat
		{
			get { return @"ServiceHostComponent.StateChange.{0}.{1}.{2}.{3}"; }
		}

		protected override string routingKeyExplanation
		{
			get { return @"ServiceHostComponent.StateChange.Machine.Environment.Component.State"; }
		}

		public string Environment { get; set; }
		public string ServiceComponent { get; set; }
		public string State { get; set; }

		public ServiceHostComponentStateChange()
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
