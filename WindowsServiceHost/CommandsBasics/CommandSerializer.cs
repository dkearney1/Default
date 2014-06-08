using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Commands
{
	public static class CommandSerializer
	{
		public static string ContentEncoding
		{
			get { return Encoding.UTF8.WebName; }
		}

		public static string ContentType
		{
			get { return "application/json"; }
		}

		public static byte[] Serialize(ICommand cmd)
		{
			if (cmd == null)
				throw new ArgumentNullException("cmd");

			return Encoding.UTF8.GetBytes(CommandSerializer.ToJson(cmd));
		}

		public static string ToJson(ICommand cmd)
		{
			if (cmd == null)
				throw new ArgumentNullException("evnt");

			return JsonConvert.SerializeObject(cmd, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}
}
