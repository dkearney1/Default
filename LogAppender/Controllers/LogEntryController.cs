using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace LogAppender.Controllers
{
	public class LogEntryController : ApiController
	{
		private static readonly char[] SPLITCHAR = new char[] { '|' };
		private static readonly string WindowsNL = "\r\n";
		private static readonly string RiscNL = "\n\r";
		private static readonly string UnixNL = "\n";
		private static readonly string MacNL = "\r";

		// POST: api/LogEntry
		public void Post([FromBody]string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return;

			Task.Factory.StartNew(() => Log(value));
		}

		private async Task Log(string value)
		{
			var logFile = ConfigurationManager.AppSettings["LogFile"];
			if (string.IsNullOrWhiteSpace(logFile))
				throw new Exception("Misconfigured Log File");

			var replacement = ConfigurationManager.AppSettings["ReplaceWith"];

			using (FileStream fs = new FileStream(logFile, FileMode.OpenOrCreate, FileSystemRights.AppendData, FileShare.Write, 4096, FileOptions.None))
			using (StreamWriter sw = new StreamWriter(fs))
			{
				sw.AutoFlush = true;

				if (!string.IsNullOrEmpty(replacement))
				{
					var sb = new StringBuilder(value);
					sb.Replace(WindowsNL, replacement);
					sb.Replace(RiscNL, replacement);
					sb.Replace(UnixNL, replacement);
					sb.Replace(MacNL, replacement);
					value = sb.ToString();
				}

				await sw.WriteLineAsync(value);
			}
		}
	}
}
