using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using LogAppender.Models;

namespace LogAppender.Controllers
{
	public class LogEntryController : ApiController
	{
		// POST: api/LogEntry
		public void Post([FromBody]string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return;

			var counter = 0;
			var added = false;
			var queue = LogEntryQueue.Instance.Queue;

			do
			{
				added = queue.TryAdd(value, TimeSpan.FromMilliseconds(10d));
				++counter;

			} while (!added && counter < 3);
		}
	}
}
