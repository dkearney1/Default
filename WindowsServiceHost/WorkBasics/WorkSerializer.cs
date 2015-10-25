using System;
using System.Text;
using Newtonsoft.Json;

namespace DKK.Work
{
	public static class WorkSerializer
	{
		public static string ContentEncoding => Encoding.UTF8.WebName;

		public static string ContentType => "application/json";

		public static byte[] Serialize(IWork work)
		{
			if (work == null)
				throw new ArgumentNullException(nameof(work));

			return Encoding.UTF8.GetBytes(WorkSerializer.ToJson(work));
		}

		public static string ToJson(IWork work)
		{
			if (work == null)
				throw new ArgumentNullException(nameof(work));

			return JsonConvert.SerializeObject(work, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}
}
