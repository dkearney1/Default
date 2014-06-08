using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomTomMapsAPITest.TomTom
{
	public class GeoPoint
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }

		public override string ToString()
		{
			return string.Format("{0:f5},{1:f5}", this.Latitude, this.Longitude);
		}
	}

	public class RoutingRequest
	{
		public enum RoutingType
		{
			Quickest,
			Shortest,
			AvoidMotorway,
			SpeedLimited,
			Green
		};

		public string Service { get; private set; }
		public int VersionNumber { get; private set; }
		public GeoPoint Start { get; private set; }
		public GeoPoint End { get; private set; }
		public List<GeoPoint> IntermediatePoints { get; set; }
		public RoutingType RouteType { get; set; }
		public string ContentType { get; set; }
		public string Key { get; private set; }
		public bool AvoidTraffic { get; set; }
		public bool IncludeTraffic { get; set; }
		public bool AvoidTolls { get; set; }
		public bool IncludeInstructions { get; set; }

		public RoutingRequest(string apiKey, GeoPoint start, GeoPoint end)
		{
			this.Service = "route";
			this.VersionNumber = 3;
			this.Start = start;
			this.End = end;
			this.ContentType = "json";
			this.Key = apiKey;
		}

		public string UriPath
		{
			get
			{
				List<GeoPoint> points = new List<GeoPoint>() { this.Start };
				points.AddRange(this.IntermediatePoints.Take(3));
				points.Add(this.End);
				
				string allPoints = string.Join(":", points);

				return string.Format("{0}/{1}/{2}/{3}/{4}/{5}",
					Constants.TomTomBasePath,	// 0
					this.Service,				// 1
					this.VersionNumber,			// 2
					allPoints,					// 3
					this.RouteType,				// 4
					this.ContentType			// 5
				);
			}
		}
	}
}
