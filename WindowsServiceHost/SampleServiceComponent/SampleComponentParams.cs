using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.SampleServiceComponent
{
	[Serializable]
	public class SampleComponentParams
	{
		public int IdleMessageRate { get; set; }
		public string WorkQueue { get; set; }
	}
}
