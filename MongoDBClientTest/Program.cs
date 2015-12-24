using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBClientTest
{
	class Program
	{
		static void Main(string[] args)
		{
			MainAsync(args).Wait();
			Console.WriteLine();
			Console.WriteLine("Press Enter to exit");
			Console.ReadLine();
		}

		static async Task MainAsync(string[] args)
		{
			var client = new MongoClient();
			var db = client.GetDatabase("test");

			var animals = db.GetCollection<BsonDocument>("animals");

			var animal = new BsonDocument
							{
							{"animal", "monkey"}
							};

			await animals.InsertOneAsync(animal);
			animal.Remove("animal");
			animal.Add("animal", "cat");
			animal["_id"] = ObjectId.GenerateNewId();
			await animals.InsertOneAsync(animal);
			animal.Remove("animal");
			animal.Add("animal", "lion");
			animal["_id"] = ObjectId.GenerateNewId();
			await animals.InsertOneAsync(animal);
		}

		private static async Task MainAsync1(string[] args)
		{
			var settings = new MongoClientSettings()
			{
				IPv6 = true,
				GuidRepresentation = GuidRepresentation.Standard,
			};

			var database = "final7";
			var albumCollection = "albums";
			var imageCollection = "images";

			var client = new MongoClient(settings);
			var db = client.GetDatabase(database);
			var albums = db.GetCollection<Album>(albumCollection);

			var nonOrphanedImageIds = new List<int>();

			await albums.Find(a => true).ForEachAsync(a => nonOrphanedImageIds.AddRange(a.images));
			Console.WriteLine("Found {0} non-orphaned image ids in the Albums collection", nonOrphanedImageIds.Count);

			var imageIdDict = nonOrphanedImageIds.ToDictionary(x => x, x => true);

			var images = db.GetCollection<Image>(imageCollection);

			var orphans = 0;

			await images.Find(i => true).ForEachAsync(async (i) => 
			{
				if (imageIdDict.ContainsKey(i._id))
					return;

				await Program.DeleteItem<Image>(settings, database, imageCollection, i._id);

				orphans++;
			});

			Console.WriteLine("Found {0} orphaned imagws in the Images collection", orphans);
		}

		private static async Task DeleteItem<T>(MongoClientSettings settings, string database, string collection, int id)
			where T : IHasId
		{
			var client = new MongoClient(settings);
			var db = client.GetDatabase(database);
			var images = db.GetCollection<T>(collection);
			await images.DeleteOneAsync(Builders<T>.Filter.Eq(x => x._id, id));
		}
	}
}
