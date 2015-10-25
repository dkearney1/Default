using System.Collections.Generic;

namespace DKK.Messaging
{
	public sealed class QueueSettings
	{
		public string Name { get; set; }
		public bool Durable { get; set; }
		public bool Exclusive { get; set; }
		public bool AutoDelete { get; set; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public IDictionary<string, object> Arguments { get; set; }
	}
}
