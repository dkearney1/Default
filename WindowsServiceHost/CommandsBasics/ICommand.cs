using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Commands
{
	public interface ICommand
	{
		DateTimeOffset Created { get; }
		string FromMachine { get; }
		string FromProcess { get; }
		string FromUserName { get; }
		string ToMachine { get; }
		string ToProcess { get; }
		string RoutingKey { get; }
		Guid? CorrelationId { get; }
		IEnumerable<Type> ResponseTypes { get; }
	}
}
