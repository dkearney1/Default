using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

		protected async Task<List<T>> ExecuteQueryAsync(FilterDefinition<T> filter, FindOptions options = null)
		{
			var mongodb = this.Connect();
			return await mongodb
								.GetCollection<T>(this.collectionName)
								.Find<T>(filter, options)
								.ToListAsync();
		}

		protected IEnumerable<T> ExecuteQuery(FilterDefinition<T> filter, FindOptions options = null)
		{
			var task = this.ExecuteQueryAsync(filter, options);
			task.Wait();
			return task.Result;
		}

		protected async Task InsertOneAsync(T item)
		{
			var mongodb = this.Connect();
			await mongodb
								.GetCollection<T>(this.collectionName)
								.InsertOneAsync(item);
		}

		protected void Insert(T item)
		{
			var task = this.InsertOneAsync(item);
			task.Wait();
		}

		protected async Task<T> FindOneAndModifyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, FindOneAndUpdateOptions<T, T> options = null)
		{
			var mongodb = this.Connect();
			return await mongodb
							  .GetCollection<T>(this.collectionName)
							  .FindOneAndUpdateAsync(filter, update, options);
		}

		protected T FindOneAndModify(FilterDefinition<T> filter, UpdateDefinition<T> update, FindOneAndUpdateOptions<T, T> options = null)
		{
			var task = this.FindOneAndModifyAsync(filter, update, options);
			task.Wait();
			return task.Result;
		}

		protected async Task<UpdateResult> FindManyAndModifyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null)
		{
			var mongodb = this.Connect();
			return await mongodb
								.GetCollection<T>(this.collectionName)
								.UpdateManyAsync(filter, update, options);
		}

		protected UpdateResult FindManyAndModify(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null)
		{
			var task = this.FindManyAndModifyAsync(filter, update, options);
			task.Wait();
			return task.Result;
		}

		protected async Task<DeleteResult> DeleteOneAsync(FilterDefinition<T> filter)
		{
			var mongodb = this.Connect();
			return await mongodb
							  .GetCollection<T>(this.collectionName)
							  .DeleteOneAsync(filter);
		}

		protected DeleteResult DeleteOne(FilterDefinition<T> filter)
		{
			var task = this.DeleteOneAsync(filter);
			task.Wait();
			return task.Result;
		}

		protected async Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter)
		{
			var mongodb = this.Connect();
			return await mongodb
								.GetCollection<T>(this.collectionName)
								.DeleteManyAsync(filter);

		}

		protected DeleteResult DeleteMany(FilterDefinition<T> filter)
		{
			var task = this.DeleteManyAsync(filter);
			task.Wait();
			return task.Result;
		}

		protected virtual void BasicGuards(T item, string paramName)
		{
			if (item == null)
				throw new ArgumentNullException(paramName);

			if (item.Id == Guid.Empty)
				throw new ArgumentOutOfRangeException(paramName, $"Id of {paramName} is not set");
		}

		protected virtual FilterDefinition<T> BuildFilter(T persisted)
		{
			var filterBuilder = Builders<T>.Filter;
			return filterBuilder.Eq(sc => sc.Id, persisted.Id) &
				filterBuilder.Eq(sc => sc.RowVersion, persisted.RowVersion);
		}

		protected virtual FindOneAndUpdateOptions<T, T> BuildOptions()
		{
			return new FindOneAndUpdateOptions<T, T>()
			{
				MaxTime = TimeSpan.FromSeconds(5d),
				ReturnDocument = ReturnDocument.After,
			};
		}
	}
}
