using System;
using System.Text;
using Newtonsoft.Json;

namespace DKK.Events
{
	public static class EventSerializer
	{
		public static string ContentEncoding => Encoding.UTF8.WebName;

		public static string ContentType => "application/json";

		public static byte[] Serialize(IEvent evnt)
		{
			if (evnt == null)
				throw new ArgumentNullException(nameof(evnt));

			return Encoding.UTF8.GetBytes(EventSerializer.ToJson(evnt));
		}

		public static string ToJson(IEvent evnt)
		{
			if (evnt == null)
				throw new ArgumentNullException(nameof(evnt));

			return JsonConvert.SerializeObject(evnt, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}
}
