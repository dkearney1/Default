using GoogleMapsAPITest.Google;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapsAPITest
{
	internal class GoogleAPITester
	{
		private readonly UriBuilder b = new UriBuilder("https", "maps.googleapis.com")
		{
			Path = "maps/api/distancematrix/json"
		};

		public GoogleAPITester()
		{
		}

		internal async Task<DistanceMatrixResponseWrapper> GetResponse(Request request)
		{
			// required parameters
			Uri uri = this.b.Uri
				.AddQuery("origins", string.Join("|", request.Origins.Values))
				.AddQuery("destinations", string.Join("|", request.Destinations.Values))
				.AddQuery("sensor", request.Sensor ? "true" : "false")
				.AddQuery("key", request.Key)
			;

			// optional parameters
			if (request.UnitOfMeasure != Request.UnitSystem.metric)
				uri = uri.AddQuery("units", Enum.GetName(typeof(Request.UnitSystem), request.UnitOfMeasure));

			if (request.Avoid != Request.Avoidance.none)
				uri = uri.AddQuery("avoid", Enum.GetName(typeof(Request.Avoidance), request.Avoid));

			if (request.DepartureTime.HasValue)
			{
				DateTime dt = new DateTime(request.DepartureTime.Value.Ticks, DateTimeKind.Utc), 
					epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				uri = uri.AddQuery("departure_time", (dt - epoc).TotalSeconds.ToString(CultureInfo.InvariantCulture));
			}

			var webRequest = WebRequest.Create(uri);

			using (var webResponse = await webRequest.GetResponseAsync())
			using (var responseStream = webResponse.GetResponseStream())
			using (var ms = new MemoryStream())
			{
				await responseStream.CopyToAsync(ms);
				string responseString = Encoding.UTF8.GetString(ms.ToArray());
				Trace.WriteLine(responseString);
				var rawResponse = fastJSON.JSON.ToObject<Response>(responseString);
				var wrappedResponse = new DistanceMatrixResponseWrapper(request, rawResponse);
				return wrappedResponse;
			}
		}
	}
}
