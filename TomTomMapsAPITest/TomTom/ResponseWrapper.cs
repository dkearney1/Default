using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TomTomMapsAPITest.TomTom
{
	class ResponseWrapper
	{
		private RoutingRequest request;
		private RoutingResponse rawResponse;

		public ResponseWrapper(RoutingRequest request, RoutingResponse rawResponse)
		{
			// TODO: Complete member initialization
			this.request = request;
			this.rawResponse = rawResponse;
		}
	}
}
