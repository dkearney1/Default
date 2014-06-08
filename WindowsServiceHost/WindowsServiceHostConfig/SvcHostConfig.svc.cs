﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DKK.WindowsServiceHostConfig
{
	public class SvcHostConfig : ISvcHostConfig
	{
		private readonly string prefix = "shc:";

		public IEnumerable<KeyValuePair<string, string>> GetConfig()
		{
			List<KeyValuePair<string, string>> response = new List<KeyValuePair<string, string>>();

			var appSettings = ConfigurationManager.AppSettings;
			for (int i = 0; i < appSettings.Count; i++)
				if (appSettings.Keys[i].StartsWith(prefix))
					response.Add(new KeyValuePair<string, string>(appSettings.Keys[i].Replace(prefix, string.Empty), appSettings[i]));

			return response;
		}
	}
}