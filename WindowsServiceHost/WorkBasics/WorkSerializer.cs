using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Work
{
	public static class WorkSerializer
	{
		public static string ContentEncoding
		{
			get { return Encoding.UTF8.WebName; }
		}

		public static string ContentType
		{
			get { return "application/json"; }
		}

		public static byte[] Serialize(IWork work)
		{
			if (work == null)
				throw new ArgumentNullException("cmd");

			return Encoding.UTF8.GetBytes(WorkSerializer.ToJson(work));
		}

		public static string ToJson(IWork work)
		{
			if (work == null)
				throw new ArgumentNullException("evnt");

			return JsonConvert.SerializeObject(work, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}
}
