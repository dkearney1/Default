using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Work
{
	public interface IWork
	{
		DateTimeOffset Created { get; }
		string ItemType { get; }
		Guid ItemId { get; }
	}
}
