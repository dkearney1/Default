using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DKK.WindowsServiceComponentConfig
{
	public class SvcComponentConfig : ISvcComponentConfig
	{
		private readonly string prefix = "scc:";

		public IEnumerable<string> GetEnvironments()
		{
			var key = $"{prefix}Environments";
			var response = new List<string>();

			var appSettings = ConfigurationManager.AppSettings;
			if (!string.IsNullOrWhiteSpace(appSettings[key]))
				response.AddRange(appSettings[key].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));

			return response;
		}

		public IEnumerable<KeyValuePair<string, string>> GetEnvironmentConfig(string environment)
		{
			var key = $"{prefix}Environment{environment}";
			var response = new List<KeyValuePair<string, string>>();

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
