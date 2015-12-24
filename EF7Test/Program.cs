using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF7Test
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var db = new DKKContext())
			{
				db.Client.Add(new Client()
				{
					ClientId = "CLIENT2",
					Name = "Client 2",
					IsActive = true
				});

				var count = db.SaveChanges();

				Console.WriteLine($"{count} records saved to database");
				Console.WriteLine();

				Console.WriteLine("All Clients in database");
				foreach (var client in db.Client)
					Console.WriteLine($" - {client.ClientId} - {client.Name}, IsActive: {client.IsActive}");
			}
		}
	}
}
