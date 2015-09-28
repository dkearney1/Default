using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.POCOProvider
{
	public abstract class PocoProviderBase<T>
		where T : POCOs.PocoBase 
	{
		protected MongoClient mongoClient;
		protected string repositoryName { get; set; }
		protected string collectionName { get; set; }
		protected ReadPreference readPreference = ReadPreference.PrimaryPreferred;
		protected IEnumerable<KeyValuePair<string, string>> MongoEnv;

		public PocoProviderBase(IEnumerable<KeyValuePair<string,string>> mongoEnv)
		{
			this.MongoEnv = mongoEnv;

			var server = this.MongoEnv.Single(kvp => kvp.Key == "MongoServer").Value;
            var port = int.Parse(this.MongoEnv.Single(kvp => kvp.Key == "MongoPort").Value);

            var conString = string.Format("mongodb://{0}:{1}", server, port);
			this.mongoClient = new MongoClient(conString);
		}

		protected MongoDatabase Connect()
		{
			var server = this.mongoClient.GetServer();
			var database = server.GetDatabase(this.repositoryName, new MongoDatabaseSettings() { ReadPreference = this.readPreference});
			return database;
		}

		protected IQueryable<T> AsQueryable()
		{
			var mongodb = this.Connect();
			return mongodb.GetCollection<T>(this.collectionName).AsQueryable<T>();
		}

		protected IEnumerable<T> ExecuteQuery(IMongoQuery query)
		{
			var mongodb = this.Connect();
			return mongodb.GetCollection<T>(this.collectionName).Find(query);
		}

		protected WriteConcernResult Insert(T item)
		{
            var mongodb = this.Connect();
			return mongodb.GetCollection<T>(this.collectionName).Insert<T>(item);
		}

		protected FindAndModifyResult FindAndModify(IMongoQuery query, IMongoUpdate update)
		{
            var mongodb = this.Connect();
			Debug.WriteLine(string.Format("Find \"{0}\", Modify \"{1}\"", query, update));
            var args = new FindAndModifyArgs()
			{
				Query = query,
				Update = update,
				Upsert = false,
				VersionReturned = FindAndModifyDocumentVersion.Modified
			};
			return mongodb.GetCollection<T>(this.collectionName).FindAndModify(args);
		}

		protected WriteConcernResult Delete(IMongoQuery query)
		{
            var mongodb = this.Connect();
			return mongodb.GetCollection<T>(this.collectionName).Remove(query);
		}
	}
}
