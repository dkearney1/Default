using System;

namespace DKK.SampleServiceComponent
{
	[Serializable]
	public class SampleComponentParams
	{
		public int IdleMessageRate { get; set; }
		public string WorkQueue { get; set; }
	}
}
