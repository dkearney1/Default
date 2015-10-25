using System;
using System.Collections.Generic;

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
