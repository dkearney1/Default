using DKK.POCOs;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.POCOProvider
{
	public sealed class ServiceHostProvider : PocoProviderBase<ServiceHost>
	{
		public ServiceHostProvider(IEnumerable<KeyValuePair<string, string>> mongoEnv)
			:base(mongoEnv)
		{
			this.repositoryName = this.MongoEnv.Single(kvp => kvp.Key == "MongoRepositorySvcConfig").Value;
			this.collectionName = "Services";
		}

		public IQueryable<ServiceHost> Queryable()
		{
			return base.AsQueryable();
		}

		public new void Insert(ServiceHost ServiceHost)
		{
			var r = base.Insert(ServiceHost);

			if (!r.Ok)
				throw new ArgumentOutOfRangeException("ServiceHost", string.Format("ServiceHost {0}, {1}", ServiceHost.Id, r.ErrorMessage));
		}

		public void Update(ServiceHost ServiceHost)
		{
			if (ServiceHost.Id == Guid.Empty)
				throw new ArgumentNullException("ServiceHost");

			int rowVersion = ServiceHost.RowVersion;

			var query = Query.And(
									Query<ServiceHost>.EQ(e => e.Id, ServiceHost.Id),
									Query<ServiceHost>.EQ(e => e.RowVersion, rowVersion)
								);

			var update = this.BuildUpdate(ServiceHost);

			var r = base.FindAndModify(query, update);

			if (!r.Ok)
				throw new ArgumentOutOfRangeException("ServiceHost", string.Format("ServiceHost {0}, {1}", ServiceHost.Id, r.ErrorMessage));

			ServiceHost.RowVersion++;
		}

		public void Delete(ServiceHost ServiceHost)
		{
			if (ServiceHost.Id == Guid.Empty)
				throw new ArgumentNullException("ServiceHost");

			int rowVersion = ServiceHost.RowVersion;

			var query = Query.And(
									Query<ServiceHost>.EQ(e => e.Id, ServiceHost.Id),
									Query<ServiceHost>.EQ(e => e.RowVersion, rowVersion)
								);

			var r = base.Delete(query);

			if (!r.Ok)
				throw new ArgumentOutOfRangeException("ServiceHost", string.Format("ServiceHost {0}, {1}", ServiceHost.Id, r.ErrorMessage));
		}

		private UpdateBuilder<ServiceHost> BuildUpdate(ServiceHost current)
		{
			var update = Update<ServiceHost>
							.Set(e => e.Machine, current.Machine)
							.Set(e => e.CommandMessageQueue, current.CommandMessageQueue)
							.Set(e => e.Components, current.Components)
							.Set(e => e.Creator, current.Creator)
							.Set(e => e.CreateDate, current.CreateDate)
							.Set(e => e.LastUpdater, current.LastUpdater)
							.Set(e => e.LastUpdate, current.LastUpdate)
							.Inc(e => e.RowVersion, 1);

			return update;
		}
	}
}
