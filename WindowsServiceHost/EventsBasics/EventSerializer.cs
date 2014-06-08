using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Events
{
	public static class EventSerializer
	{
		public static string ContentEncoding
		{
			get { return Encoding.UTF8.WebName; }
		}

		public static string ContentType
		{
			get { return "application/json"; }
		}

		public static byte[] Serialize(IEvent evnt)
		{
			if (evnt == null)
				throw new ArgumentNullException("evnt");

			return Encoding.UTF8.GetBytes(EventSerializer.ToJson(evnt));
		}

		public static string ToJson(IEvent evnt)
		{
			if (evnt == null)
				throw new ArgumentNullException("evnt");

			return JsonConvert.SerializeObject(evnt, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}
}
