using DKK.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ServiceHostEvents
{
	[Serializable]
	public sealed class ServiceComponentStatus : EventBase
	{
		protected override string routingKeyFormat
		{
			get { return @"ServiceComponent.StateChange.{0}.{1}.{2}"; }
		}

		protected override string routingKeyExplanation
		{
			get { return @"ServiceComponent.StateChange.Machine.ServiceComponent.Status"; }
		}

		public string ServiceComponent { get; set; }
		private string _status;
		public string Status
		{
			get { return this._status; }
			set { this._status = value; this.StatusTime = DateTimeOffset.Now; }
		}
		public DateTimeOffset StatusTime { get; set; }
		public TimeSpan TimeInStatus { get { return DateTimeOffset.Now - this.StatusTime; } }
		private string _subStatus;
		public string SubStatus
		{
			get { return this._subStatus; }
			set { this._subStatus = value; this.SubStatusTime = DateTimeOffset.Now; }
		}
		public DateTimeOffset SubStatusTime { get; set; }
		public TimeSpan TimeInSubStatus { get { return DateTimeOffset.Now - this.SubStatusTime; } }

		public ServiceComponentStatus()
			: base()
		{
			this.StatusTime = DateTimeOffset.MinValue;
			this.SubStatusTime = DateTimeOffset.MinValue;
		}

		public override string RoutingKey
		{
			get { return string.Format(CultureInfo.InvariantCulture, this.routingKeyFormat, this.Machine, this.ServiceComponent, this.Status); }
		}
	}
}
