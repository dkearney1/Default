using System.Text;
using Newtonsoft.Json;

namespace DKK.Commands
{
	public static class CommandDeserializer
	{
		public static object Deserialize(byte[] msg)
		{
			var s = Encoding.UTF8.GetString(msg);
			return JsonConvert.DeserializeObject(s, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}

	public static class CommandDeserializer<T>
		where T : ICommand
	{
		public static T Deserialize(byte[] msg)
		{
			var s = Encoding.UTF8.GetString(msg);
			return JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}
}
