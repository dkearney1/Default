using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Events
{
	public interface IEvent
	{
		DateTimeOffset Created { get; }
		string Machine { get; }
		string Process { get; }
		string UserName { get; }
		string RoutingKey { get; }
		string RoutingKeyExplanation { get; }
	}
}
