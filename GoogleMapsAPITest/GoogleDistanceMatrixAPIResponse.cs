using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapsAPITest.Google
{
	public class Distance
	{
		public string text { get; set; }
		public int value { get; set; }
	}

	public class Duration
	{
		public string text { get; set; }
		public int value { get; set; }
	}

	public class Element
	{
		public Distance distance { get; set; }
		public Duration duration { get; set; }
		public string status { get; set; }
	}

	public class Row
	{
		public List<Element> elements { get; set; }
	}

	public class Response
	{
		public List<string> destination_addresses { get; set; }
		public List<string> origin_addresses { get; set; }
		public List<Row> rows { get; set; }
		public string status { get; set; }
		public string error_message { get; set; }
	}
}

//{
//   "destination_addresses" : [
//	  "Dallas/Fort Worth International Airport, 3200 East Airfield Drive, DFW Airport, TX 75261, USA"
//   ],
//   "origin_addresses" : [ "2109 Canyon Valley Trail, Plano, TX 75023, USA" ],
//   "rows" : [
//	  {
//		 "elements" : [
//			{
//			   "distance" : {
//				  "text" : "26.3 mi",
//				  "value" : 42277
//			   },
//			   "duration" : {
//				  "text" : "29 mins",
//				  "value" : 1713
//			   },
//			   "status" : "OK"
//			}
//		 ]
//	  }
//   ],
//   "status" : "OK"
//}