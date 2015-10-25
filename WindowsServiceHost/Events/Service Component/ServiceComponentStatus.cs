using System;
using System.Globalization;
using DKK.Events;

namespace DKK.ServiceHostEvents
{
	[Serializable]
	public sealed class ServiceComponentStatus : EventBase
	{
		protected override string routingKeyFormat => @"ServiceComponent.StateChange.{0}.{1}.{2}";

		protected override string routingKeyExplanation => @"ServiceComponent.StateChange.Machine.ServiceComponent.Status";

		public string ServiceComponent { get; set; }
		private string _status;
		public string Status
		{
			get { return this._status; }
			set { this._status = value; this.StatusTime = DateTimeOffset.Now; }
		}
		public DateTimeOffset StatusTime { get; set; }
		public TimeSpan TimeInStatus => DateTimeOffset.Now - this.StatusTime;
		private string _subStatus;
		public string SubStatus
		{
			get { return this._subStatus; }
			set { this._subStatus = value; this.SubStatusTime = DateTimeOffset.Now; }
		}
		public DateTimeOffset SubStatusTime { get; set; }
		public TimeSpan TimeInSubStatus => DateTimeOffset.Now - this.SubStatusTime;

		public ServiceComponentStatus()
			: base()
		{
			this.StatusTime = DateTimeOffset.MinValue;
			this.SubStatusTime = DateTimeOffset.MinValue;
		}

		public override string RoutingKey => string.Format(CultureInfo.InvariantCulture, this.routingKeyFormat, this.Machine, this.ServiceComponent, this.Status);
	}
}
