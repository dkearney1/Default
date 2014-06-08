using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Events
{
	public interface IQueueChangeEvent : IEvent
	{
		string FromQueue { get; }
		string ToQueue { get; }
		string Type { get; }
	}
}
