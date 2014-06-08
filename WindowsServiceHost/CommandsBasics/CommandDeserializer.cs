﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Commands
{
	public static class CommandDeserializer
	{
		public static object Deserialize(byte[] msg)
		{
			string s = Encoding.UTF8.GetString(msg);
			return JsonConvert.DeserializeObject(s, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}

	public static class CommandDeserializer<T>
		where T : ICommand
	{
		public static T Deserialize(byte[] msg)
		{
			string s = Encoding.UTF8.GetString(msg);
			return JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
		}
	}
}