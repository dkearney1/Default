using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DKK.WindowsServiceComponentConfig
{
	public class SvcComponentConfig : ISvcComponentConfig
	{
		private readonly string prefix = "scc:";

		public IEnumerable<string> GetEnvironments()
		{
			string key = string.Format("{0}Environments", prefix);
			List<string> response = new List<string>();

			var appSettings = ConfigurationManager.AppSettings;
			if (!string.IsNullOrWhiteSpace(appSettings[key]))
				response.AddRange(appSettings[key].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));

			return response;
		}

		public IEnumerable<KeyValuePair<string, string>> GetEnvironmentConfig(string environment)
		{
			string key = string.Format("{0}Environment{1}", prefix, environment);
			List<KeyValuePair<string, string>> response = new List<KeyValuePair<string, string>>();

			var appSettings = ConfigurationManager.AppSettings;
			if (!string.IsNullOrWhiteSpace(appSettings[key]))
				response.AddRange(
					appSettings[key]
						.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(kvp =>
						{
							string[] parts = kvp.Split(new char[] { '=' });
							return new KeyValuePair<string, string>(parts.First(), parts.Last());
						}));

			return response;
		}
	}
}
