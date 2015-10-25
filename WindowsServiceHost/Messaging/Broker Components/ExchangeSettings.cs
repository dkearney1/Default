using System.Collections.Generic;

namespace DKK.Messaging
{
	public sealed class ExchangeSettings
	{
		public string Name { get; set; }
		public string ExchangeType { get; set; }
		public bool Durable { get; set; }
		public bool AutoDelete { get; set; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public IDictionary<string, object> Arguments { get; set; }
	}
}
