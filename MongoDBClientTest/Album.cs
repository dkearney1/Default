using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBClientTest
{
	class Album : IHasId
	{
		public int _id { get; set; }
		public List<int> images { get; set; }
	}
}
