using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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

		public PocoProviderBase(IEnumerable<KeyValuePair<string, string>> mongoEnv)
		{
			this.MongoEnv = mongoEnv;

			var server = this.MongoEnv.Single(kvp => kvp.Key == "MongoServer").Value;
			var port = int.Parse(this.MongoEnv.Single(kvp => kvp.Key == "MongoPort").Value);

			var conString = $"mongodb://{server}:{port}";
			this.mongoClient = new MongoClient(conString);
		}

		protected IMongoDatabase Connect()
		{
			var database = this.mongoClient.GetDatabase(this.repositoryName, new MongoDatabaseSettings() { ReadPreference = this.readPreference });
			return database;
		}

		protected IQueryable<T> AsQueryable()
		{
			var mongodb = this.Connect();
			var collection = mongodb.GetCollection<T>(this.collectionName);
			return mongodb.GetCollection<T>(this.collectionName).AsQueryable<T>();
		}

		protected IEnumerable<T> ExecuteQuery(FilterDefinition<T> filter, FindOptions options = null)
		{
			var mongodb = this.Connect();
			return mongodb
						.GetCollection<T>(this.collectionName)
						.Find<T>(filter, options)
						.ToListAsync()
						.Result
						;
		}

		protected void Insert(T item)
		{
			var mongodb = this.Connect();
			mongodb
					.GetCollection<T>(this.collectionName)
					.InsertOneAsync(item)
					.RunSynchronously();
		}

		protected void FindOneAndModify(FilterDefinition<T> filter, UpdateDefinition<T> update, FindOneAndUpdateOptions<T, T> options = null)
		{
			var mongodb = this.Connect();
			mongodb
					.GetCollection<T>(this.collectionName)
					.FindOneAndUpdateAsync(filter, update, options)
					.RunSynchronously();
		}

		protected void DeleteOne(FilterDefinition<T> filter)
		{
			var mongodb = this.Connect();
			mongodb
					.GetCollection<T>(this.collectionName)
					.DeleteOneAsync(filter)
					.RunSynchronously();
		}

		protected void DeleteMany(FilterDefinition<T> filter)
		{
			var mongodb = this.Connect();
			mongodb
					.GetCollection<T>(this.collectionName)
					.DeleteManyAsync(filter)
					.RunSynchronously();
		}

		protected void BasicGuards(T item, string paramName)
		{
			if (item == null)
				throw new ArgumentNullException(paramName);

			if (item.Id == Guid.Empty)
				throw new ArgumentOutOfRangeException(paramName, $"Id of {paramName} is not set");
		}
	}
}
