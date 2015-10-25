using System;

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
