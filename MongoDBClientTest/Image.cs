using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBClientTest
{
	class Image : IHasId
	{
		public int _id { get; set; }
		public int height { get; set; }
		public int width { get; set; }
		public List<string> tags { get; set; }
	}
}
