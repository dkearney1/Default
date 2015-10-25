using System;
using DKK.Work;

namespace DKK.SampleServiceComponent
{
	[Serializable]
	public sealed class SampleWorkItem : WorkBase
	{
		public SampleWorkItem()
			: base()
		{
			this.ItemType = "SampleWorkItem";
			this.ItemId = Guid.NewGuid();
			var ticksToday = DateTime.Now.Ticks - DateTime.Today.Ticks;
			var ts = new TimeSpan(ticksToday);
			this.WorkDelay = TimeSpan.FromMilliseconds(new Random(Convert.ToInt32(ticksToday/TimeSpan.TicksPerMillisecond)).Next(1000, 10000));
		}

		public TimeSpan WorkDelay { get; }
	}
}
