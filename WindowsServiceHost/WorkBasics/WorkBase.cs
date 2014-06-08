using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Work
{
	[Serializable]
	public abstract class WorkBase : IWork
	{
		public WorkBase()
		{
			this.Created = DateTimeOffset.Now;
		}

		public DateTimeOffset Created { get; protected set; }
		public string ItemType { get; protected set; }
		public Guid ItemId { get; protected set; }
	}
}
