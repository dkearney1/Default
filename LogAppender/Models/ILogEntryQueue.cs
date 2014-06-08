using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAppender.Models
{
	internal interface ILogEntryQueue
	{
		BlockingCollection<string> Queue { get; }
	}
}
