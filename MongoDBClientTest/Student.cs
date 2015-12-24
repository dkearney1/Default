using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MongoDBClientTest
{
	class Student
	{
		public int _id { get; set; }
		public string name { get; set; }
		public List<Score> scores { get; set; }
	}
}
