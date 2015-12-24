using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMQClientTest
{
	class Program
	{
		static void Main(string[] args)
		{
			IConnection con = new ConnectionFactory().CreateConnection();
		}
	}
}
