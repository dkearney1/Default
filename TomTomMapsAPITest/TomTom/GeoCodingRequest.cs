using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomTomMapsAPITest.TomTom
{
	public class GeoCodingRequest
	{
		public string Interface { get; set; }
		public string Key { get; private set; }
		public int MaxResults { get; set; }
		public string Query { get; set; }
		public string CountryCode { get; set; }

		public GeoCodingRequest(string apiKey)
		{
			this.Key = apiKey;
		}
	}
}
