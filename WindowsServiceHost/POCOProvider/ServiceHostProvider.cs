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

		public void Update(ServiceHost serviceHost)
		{
			this.BasicGuards(serviceHost, nameof(serviceHost));

			var filter = this.BuildFilter(serviceHost);
			var update = this.BuildUpdate(serviceHost);

			base.FindOneAndModify(filter, update);

			serviceHost.RowVersion++;
		}

		public void Delete(ServiceHost serviceHost)
		{
			this.BasicGuards(serviceHost, nameof(serviceHost));

			var filter = this.BuildFilter(serviceHost);

			base.DeleteOne(filter);
		}

		private FilterDefinition<ServiceHost> BuildFilter(ServiceHost serviceHost)
		{
			var filterBuilder = Builders<ServiceHost>.Filter;
			return filterBuilder.Eq(sc => sc.Id, serviceHost.Id) &
				filterBuilder.Eq(sc => sc.RowVersion, serviceHost.RowVersion);
		}

		private UpdateDefinition<ServiceHost> BuildUpdate(ServiceHost current)
		{
			var updateBuilder = Builders<ServiceHost>.Update;
			return updateBuilder
							.Set(e => e.Machine, current.Machine)
							.Set(e => e.CommandMessageQueue, current.CommandMessageQueue)
							.Set(e => e.Components, current.Components)
							.Set(e => e.Creator, current.Creator)
							.Set(e => e.CreateDate, current.CreateDate)
							.Set(e => e.LastUpdater, current.LastUpdater)
							.Set(e => e.LastUpdate, current.LastUpdate)
							.Inc(e => e.RowVersion, 1);
		}
	}
}
