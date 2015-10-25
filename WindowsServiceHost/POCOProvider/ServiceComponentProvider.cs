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
			:base(mongoEnv)
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

		public void Update(ServiceComponent serviceComponent)
		{
			this.BasicGuards(serviceComponent, nameof(serviceComponent));

			var filter = this.BuildFilter(serviceComponent);
			var update = this.BuildUpdate(serviceComponent);

			base.FindOneAndModify(filter, update);

			serviceComponent.RowVersion++;
		}

		public void Delete(ServiceComponent serviceComponent)
		{
			this.BasicGuards(serviceComponent, nameof(serviceComponent));

			var filter = this.BuildFilter(serviceComponent);

			base.DeleteOne(filter);
		}

		private FilterDefinition<ServiceComponent> BuildFilter(ServiceComponent serviceComponent)
		{
			var filterBuilder = Builders<ServiceComponent>.Filter;
			return filterBuilder.Eq(sc => sc.Id, serviceComponent.Id) &
				filterBuilder.Eq(sc => sc.RowVersion, serviceComponent.RowVersion);
		}

		private UpdateDefinition<ServiceComponent> BuildUpdate(ServiceComponent current)
		{
			var updateBuilder = Builders<ServiceComponent>.Update;
			return updateBuilder
							.Set(e => e.Assembly, current.Assembly)
							.Set(e => e.Class, current.Class)
							.Set(e => e.ParamsAssembly, current.ParamsAssembly)
							.Set(e => e.ParamsClass, current.ParamsClass)
							.Set(e => e.Config, current.Config)
							.Set(e => e.IsActive, current.IsActive)
							.Set(e => e.IsPaused, current.IsPaused)
							.Set(e => e.Creator, current.Creator)
							.Set(e => e.CreateDate, current.CreateDate)
							.Set(e => e.LastUpdater, current.LastUpdater)
							.Set(e => e.LastUpdate, current.LastUpdate)
							.Inc(e => e.RowVersion, 1);
		}
	}
}
