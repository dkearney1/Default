using System.Text;
using Newtonsoft.Json;

namespace DKK.Events
{
	public static class EventDeserializer
	{
		public static object Deserialize(byte[] msg)
		{
			var s = Encoding.UTF8.GetString(msg);
			return JsonConvert.DeserializeObject(s, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}

	public static class EventDeserializer<T>
		 where T : IEvent
	{
		public static T Deserialize(byte[] msg)
		{
			var s = Encoding.UTF8.GetString(msg);
			return JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}
}
