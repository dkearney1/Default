using System;
using System.Text;
using Newtonsoft.Json;

namespace DKK.Commands
{
	public static class CommandSerializer
	{
		public static string ContentEncoding => Encoding.UTF8.WebName;

		public static string ContentType => "application/json";

		public static byte[] Serialize(ICommand cmd)
		{
			if (cmd == null)
				throw new ArgumentNullException(nameof(cmd));

			return Encoding.UTF8.GetBytes(CommandSerializer.ToJson(cmd));
		}

		public static string ToJson(ICommand cmd)
		{
			if (cmd == null)
				throw new ArgumentNullException(nameof(cmd));

			return JsonConvert.SerializeObject(cmd, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}
}
