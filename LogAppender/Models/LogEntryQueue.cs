using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace LogAppender.Models
{
	internal sealed class LogEntryQueue : ILogEntryQueue
	{
		#region Singleton implementation
		private static readonly Lazy<LogEntryQueue> lazy = new Lazy<LogEntryQueue>(() => new LogEntryQueue());
		public static LogEntryQueue Instance { get { return lazy.Value; } }
		private LogEntryQueue()
		{
		}
		#endregion

		#region ILogEntryQueue implementation
		private static readonly BlockingCollection<string> _queue = new BlockingCollection<string>();
		public BlockingCollection<string> Queue { get { return _queue; } }
		#endregion

		private static readonly char[] SPLITCHAR = new char[] { '|' };
		private static readonly string WindowsNL = "\r\n";
		private static readonly string UnixNL = "\n";
		private static readonly string MacNL = "\r";
		private static readonly string RiscNL = "\n\r";

		private static readonly Task _queueProcessor = Task.Factory.StartNew(QueueProcessorTask);

		private static void QueueProcessorTask()
		{
			for (; ; )
			{
				while (LogEntryQueue._queue.Count == 0)
					Thread.Sleep(TimeSpan.FromMilliseconds(10d));

				var logFile = ConfigurationManager.AppSettings["LogFile"];
				if (string.IsNullOrWhiteSpace(logFile))
					throw new Exception("Misconfigured Log File");

				var replacement = ConfigurationManager.AppSettings["ReplaceWith"];

				using (StreamWriter sw = new StreamWriter(logFile, true))
				{
					while (LogEntryQueue._queue.Count > 0)
					{
						string logEntry;
						if (LogEntryQueue._queue.TryTake(out logEntry, TimeSpan.FromMilliseconds(10d)))
						{
							if (!string.IsNullOrEmpty(replacement))
							{
								var sb = new StringBuilder(logEntry);
								sb.Replace(WindowsNL, replacement);
								sb.Replace(RiscNL, replacement);
								sb.Replace(UnixNL, replacement);
								sb.Replace(MacNL, replacement);
								logEntry = sb.ToString();
							}

							sw.WriteLine(logEntry);
						}
					}
				}
			}
		}
	}
}