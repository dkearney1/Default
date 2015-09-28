using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Work
{
	public static class WorkDeserializer
	{
		public static object Deserialize(byte[] msg)
		{
			var s = Encoding.UTF8.GetString(msg);
			return JsonConvert.DeserializeObject(s, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}

	public static class WorkDeserializer<T>
		where T: IWork
	{
		public static T Deserialize(byte[] msg)
		{
			var s = Encoding.UTF8.GetString(msg);
			return JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}
}
