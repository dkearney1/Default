using System;
using System.Collections.Generic;
using System.Linq;
using DKK.POCOs;
using MongoDB.Driver;

namespace DKK.POCOProvider
{
	public sealed class ServiceComponentProvider : PocoProviderBase<ServiceComponent>
	{
		public ServiceComponentProvider(IEnumerable<KeyValuePair<string, string>> mongoEnv)
			: base(mongoEnv)
		{
			this.repositoryName = this.MongoEnv.Single(kvp => kvp.Key == "MongoRepositorySvcConfig").Value;
			this.collectionName = "Services";
		}

		public IQueryable<ServiceComponent> Queryable() => base.AsQueryable();

		public new void Insert(ServiceComponent serviceComponent)
		{
			this.BasicGuards(serviceComponent, nameof(serviceComponent));

			base.Insert(serviceComponent);
		}

		public ServiceComponent Update(ServiceComponent serviceComponent)
		{
			this.BasicGuards(serviceComponent, nameof(serviceComponent));

			var filter = this.BuildFilter(serviceComponent);
			var update = this.BuildUpdate(serviceComponent);
			var options = this.BuildOptions();

			return base.FindOneAndModify(filter, update, options);
		}

		public void Delete(ServiceComponent serviceComponent)
		{
			this.BasicGuards(serviceComponent, nameof(serviceComponent));

			var filter = this.BuildFilter(serviceComponent);
			var result = base.DeleteOne(filter);

			if (!result.IsAcknowledged || result.DeletedCount != 1)
				throw new POCOItemUpdatedException($"No matching record found for {serviceComponent.FriendlyName} by Id and RowVersion.");
		}

		private UpdateDefinition<ServiceComponent> BuildUpdate(ServiceComponent update)
		{
			var updateBuilder = Builders<ServiceComponent>.Update;
			return updateBuilder
							// set most properties to the update's value
							.Set(persisted => persisted.Assembly, update.Assembly)
							.Set(persisted => persisted.Class, update.Class)
							.Set(persisted => persisted.ParamsAssembly, update.ParamsAssembly)
							.Set(persisted => persisted.ParamsClass, update.ParamsClass)
							.Set(persisted => persisted.Config, update.Config)
							.Set(persisted => persisted.IsActive, update.IsActive)
							.Set(persisted => persisted.IsPaused, update.IsPaused)
							.Set(persisted => persisted.Creator, update.Creator)
							.Set(persisted => persisted.CreateDate, update.CreateDate)
							.Set(persisted => persisted.LastUpdater, update.LastUpdater)
							.Set(persisted => persisted.LastUpdate, update.LastUpdate)
							// increment the RowVersion
							.Inc(persisted => persisted.RowVersion, 1);
		}
	}
}
