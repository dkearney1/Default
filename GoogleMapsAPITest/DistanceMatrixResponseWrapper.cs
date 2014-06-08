using GoogleMapsAPITest.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapsAPITest
{
	public class SeparationDetails
	{
		public string Status { get; set; }
		public string DistanceText { get; set; }
		public int DistanceValue { get; set; }
		public string DurationText { get; set; }
		public int DurationValue { get; set; }
	}
	public class DistanceMatrixResponseWrapper
	{
		public readonly string GoogleOKResponse = "OK";
		public Request Request { get; private set; }
		public Response Response { get; private set; }

		public DistanceMatrixResponseWrapper(Request request, Response response)
		{
			this.Request = request;
			this.Response = response;
		}

		public bool IsValid
		{
			get { return this.Response.status == GoogleOKResponse; }
		}

		public string GetGeocoderFormattedOrigin(string origin)
		{
			int originIndex = this.GetOriginIndex(origin);
			return this.Response.origin_addresses.ElementAt(originIndex);
		}

		public string GetGeocoderFormattedDestination(string destination)
		{
			int destinationIndex = this.GetDestinationIndex(destination);
			return this.Response.destination_addresses.ElementAt(destinationIndex);
		}

		public string GetClosestOriginToDestination(string destination)
		{
			int destinationIndex = this.GetDestinationIndex(destination);

			// find the smallest time to that destination
			int shortestTime = this.Response.rows
											.Select(e => e.elements.ElementAt(destinationIndex))
											.Where(e => e.status == GoogleOKResponse)
											.Min(e => e.duration.value);

			// find all the shortest distance to that destination
			int shortestDistance = this.Response.rows
											.Where(r => r.elements.ElementAt(destinationIndex).duration.value == shortestTime)
											.Select(r => r.elements.ElementAt(destinationIndex))
											.Min(e => e.distance.value);

			// from the origins with the shortestTime
			// then from the origins with the shortestDistance
			// select the 1st (highest preference) origin
			int originIndex = 0;
			foreach (var row in this.Response.rows)
			{
				Element el = row.elements.ElementAt(destinationIndex);
				if (el.status == GoogleOKResponse && el.duration.value == shortestTime && el.distance.value == shortestDistance)
					break;
				++originIndex;
			}

			return this.Request.Origins.ElementAt(originIndex).Key;
		}

		public SeparationDetails GetSeparationDetails(string origin, string destination)
		{
			if (!this.Request.Origins.ContainsKey(origin))
				throw new ArgumentOutOfRangeException("origin");

			if (!this.Request.Destinations.ContainsKey(destination))
				throw new ArgumentOutOfRangeException("destination");

			int originIndex = this.GetOriginIndex(origin),
				destinationIndex = this.GetDestinationIndex(destination);

			Element el = this.Response.rows.ElementAt(originIndex).elements.ElementAt(destinationIndex);
			return new SeparationDetails()
			{
				Status = el.status,
				DistanceText = el.distance.text,
				DistanceValue = el.distance.value,
				DurationText = el.duration.text,
				DurationValue = el.duration.value
			};
		}

		private int GetOriginIndex(string origin)
		{
			return this.GetIndex(origin, this.Request.Origins.Keys);
		}

		private int GetDestinationIndex(string destination)
		{
			return this.GetIndex(destination, this.Request.Destinations.Keys);
		}

		private int GetIndex(string key, Dictionary<string, string>.KeyCollection keys)
		{
			int index = 0;
			foreach (var item in keys)
			{
				if (item == key)
					return index;
				++index;
			}
			throw new ArgumentOutOfRangeException("key");
		}
	}
}
