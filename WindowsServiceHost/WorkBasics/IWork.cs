using System;

namespace DKK.Work
{
	public interface IWork
	{
		DateTimeOffset Created { get; }
		string ItemType { get; }
		Guid ItemId { get; }
	}
}
