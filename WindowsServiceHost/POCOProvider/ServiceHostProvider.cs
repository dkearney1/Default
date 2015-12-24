using System;
using System.Collections.Generic;
using System.Linq;
using DKK.POCOs;
using MongoDB.Driver;

namespace DKK.POCOProvider
{
	public sealed class ServiceHostProvider : PocoProviderBase<ServiceHost>
	{
		public ServiceHostProvider(IEnumerable<KeyValuePair<string, string>> mongoEnv)
			: base(mongoEnv)
		{
			this.repositoryName = this.MongoEnv.Single(kvp => kvp.Key == "MongoRepositorySvcConfig").Value;
			this.collectionName = "Services";
		}

		public IQueryable<ServiceHost> Queryable() => base.AsQueryable();

		public new void Insert(ServiceHost serviceHost)
		{
			this.BasicGuards(serviceHost, nameof(serviceHost));

			base.Insert(serviceHost);
		}

		public ServiceHost Update(ServiceHost serviceHost)
		{
			this.BasicGuards(serviceHost, nameof(serviceHost));

			var filter = this.BuildFilter(serviceHost);
			var update = this.BuildUpdate(serviceHost);
			var options = this.BuildOptions();

			return base.FindOneAndModify(filter, update, options);
		}

		public void Delete(ServiceHost serviceHost)
		{
			this.BasicGuards(serviceHost, nameof(serviceHost));

			var filter = this.BuildFilter(serviceHost);
			var result = base.DeleteOne(filter);

			if (!result.IsAcknowledged || result.DeletedCount != 1)
				throw new POCOItemUpdatedException($"No matching record found for {serviceHost.Machine} by Id and RowVersion.");
		}

		private UpdateDefinition<ServiceHost> BuildUpdate(ServiceHost update)
		{
			var updateBuilder = Builders<ServiceHost>.Update;
			return updateBuilder
							// set most properties to the update's value
							.Set(persisted => persisted.Machine, update.Machine)
							.Set(persisted => persisted.CommandMessageQueue, update.CommandMessageQueue)
							.Set(persisted => persisted.Components, update.Components)
							.Set(persisted => persisted.Creator, update.Creator)
							.Set(persisted => persisted.CreateDate, update.CreateDate)
							.Set(persisted => persisted.LastUpdater, update.LastUpdater)
							.Set(persisted => persisted.LastUpdate, update.LastUpdate)
							// increment the RowVersion
							.Inc(persisted => persisted.RowVersion, 1);
		}
	}
}
