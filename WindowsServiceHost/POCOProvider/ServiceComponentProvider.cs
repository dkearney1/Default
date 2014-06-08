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
	public sealed class ServiceComponentProvider : PocoProviderBase<ServiceComponent>
	{
		public ServiceComponentProvider(IEnumerable<KeyValuePair<string, string>> mongoEnv)
			:base(mongoEnv)
		{
			this.repositoryName = this.MongoEnv.Single(kvp => kvp.Key == "MongoRepositorySvcConfig").Value;
			this.collectionName = "Services";
		}

		public IQueryable<ServiceComponent> Queryable()
		{
			return base.AsQueryable();
		}

		public new void Insert(ServiceComponent serviceComponent)
		{
			if (serviceComponent.Id == Guid.Empty)
				throw new ArgumentNullException("ServiceComponent");

			var r = base.Insert(serviceComponent);

			if (!r.Ok)
				throw new ArgumentOutOfRangeException("serviceComponent", string.Format("ServiceComponent {0}, {1}", serviceComponent.Id, r.ErrorMessage));
		}

		public void Update(ServiceComponent serviceComponent)
		{
			if (serviceComponent.Id == Guid.Empty)
				throw new ArgumentNullException("ServiceComponent");

			int rowVersion = serviceComponent.RowVersion;

			var query = Query.And(
									Query<ServiceComponent>.EQ(e => e.Id, serviceComponent.Id),
									Query<ServiceComponent>.EQ(e => e.RowVersion, rowVersion)
								);

			var update = this.BuildUpdate(serviceComponent);

			var r = base.FindAndModify(query, update);

			if (!r.Ok)
				throw new ArgumentOutOfRangeException("serviceComponent", string.Format("ServiceComponent {0}, {1}", serviceComponent.Id, r.ErrorMessage));

			serviceComponent.RowVersion++;
		}

		public void Delete(ServiceComponent serviceComponent)
		{
			if (serviceComponent.Id == Guid.Empty)
				throw new ArgumentNullException("ServiceComponent");

			int rowVersion = serviceComponent.RowVersion;

			var query = Query.And(
									Query<ServiceComponent>.EQ(e => e.Id, serviceComponent.Id),
									Query<ServiceComponent>.EQ(e => e.RowVersion, rowVersion)
								);

			var r = base.Delete(query);

			if (!r.Ok)
				throw new ArgumentOutOfRangeException("serviceComponent", string.Format("ServiceComponent {0}, {1}", serviceComponent.Id, r.ErrorMessage));
		}

		private UpdateBuilder<ServiceComponent> BuildUpdate(ServiceComponent current)
		{
			var update = Update<ServiceComponent>
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

			return update;
		}
	}
}
