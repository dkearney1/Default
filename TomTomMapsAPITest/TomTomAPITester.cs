using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TomTomMapsAPITest.TomTom;

namespace TomTomMapsAPITest
{
	internal class TomTomAPITester
	{
		private readonly UriBuilder b = new UriBuilder("https", "maps.googleapis.com");

		public TomTomAPITester()
		{
		}

		internal async Task<ResponseWrapper> GetResponse(RoutingRequest request)
		{
			this.b.Path = request.UriPath;
			Uri uri = this.b.Uri;

			uri = uri.AddQuery("key", request.Key);

			if (request.IncludeTraffic)
			{
				uri = uri.AddQuery("includeTraffic", "true");
				if (request.AvoidTraffic)
					uri = uri.AddQuery("avoidTraffic", "true");
			}

			if (request.AvoidTolls)
				uri = uri.AddQuery("avoidTolls", "true");

			if (!request.IncludeInstructions)
				uri = uri.AddQuery("includeInstructions", "false");

			var webRequest = WebRequest.Create(uri);

			using (var webResponse = await webRequest.GetResponseAsync())
			using (var responseStream = webResponse.GetResponseStream())
			using (var ms = new MemoryStream())
			{
				await responseStream.CopyToAsync(ms);
				string responseString = Encoding.UTF8.GetString(ms.ToArray());
				Trace.WriteLine(responseString);
				var rawResponse = fastJSON.JSON.ToObject<RoutingResponse>(responseString);
				var wrappedResponse = new ResponseWrapper(request, rawResponse);
				return wrappedResponse;
			}
		}
	}
}
