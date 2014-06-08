using GoogleMapsAPITest.Google;
using System;
using System.Diagnostics;
using System.Linq;

namespace GoogleMapsAPITest
{
	class Program
	{
		static void Main(string[] args)
		{
			var t = new GoogleAPITester();

			Request request = new Request(Constants.GoogleAPIKey);
			request.AddOrigin("Driver 1", "14 King St,07874");
			request.AddOrigin("Driver 2", "27 Cambridge Rd,07044");
			request.AddDestination("Fare 1", "EWR");

			var wrappedResponse = t.GetResponse(request).Result;

			if (wrappedResponse.IsValid)
			{
				foreach (var origin in request.Origins.Keys)
				{
					foreach (var destination in request.Destinations.Keys)
					{
						var details = wrappedResponse.GetSeparationDetails(origin, destination);

						if (details.Status == wrappedResponse.GoogleOKResponse)
						{
							Trace.WriteLine(string.Format("From: {1} at {2}{0}To: {3} at {4}{0}{5} in {6}{0}",
								Environment.NewLine,
								origin,
								wrappedResponse.GetGeocoderFormattedOrigin(origin),
								destination,
								wrappedResponse.GetGeocoderFormattedDestination(destination),
								details.DistanceText,
								details.DurationText
							));
						}
						else
						{
							Trace.WriteLine(string.Format("Failed to find route between {1} and {2}{0}{3}",
								Environment.NewLine,
								origin,
								destination,
								details.Status
							));
						}
					}
				}

				foreach (string destination in request.Destinations.Keys)
				{
					string closestOrigin = wrappedResponse.GetClosestOriginToDestination(destination);
					Trace.WriteLine(string.Format("{0}, starting at {1}, is closest to {2}.",
						closestOrigin,
						wrappedResponse.Request.Origins[closestOrigin],
						wrappedResponse.Request.Destinations[destination]
					));

				}

				Trace.WriteLine(string.Empty);
			}
		}
	}
}
