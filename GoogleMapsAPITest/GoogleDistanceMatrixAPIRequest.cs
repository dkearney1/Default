using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapsAPITest.Google
{
	public class Request
	{
		public enum TransportMode
		{
			driving,
			walking,
			bicycling
		};

		public enum Avoidance
		{
			none,
			tolls,
			highways,
			ferries
		};

		public enum UnitSystem
		{
			imperial,
			metric
		};

		public Dictionary<string, string> Origins { get; set; }

		public Dictionary<string, string> Destinations { get; set; }
		
		public bool Sensor { get; set; }
		
		public string Key { get; private set; }
		
		public TransportMode TransportationMode { get; set; }
		
		//public string Language { get; set; }
		
		public Avoidance Avoid { get; set; }
		
		public UnitSystem UnitOfMeasure { get; set; }
		
		public DateTime? DepartureTime { get; set; }

		public Request(string apiKey)
		{
			this.Origins = new Dictionary<string, string>();
			this.Destinations = new Dictionary<string, string>();
			this.Key = apiKey;
		}

		public void AddOrigin(string name, string origin)
		{
			this.Origins.Add(name, origin);
		}

		public void AddDestination(string name, string destination)
		{
			this.Destinations.Add(name, destination);
		}
	}
}
