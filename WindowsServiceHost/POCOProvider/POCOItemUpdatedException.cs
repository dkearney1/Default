using System;

namespace DKK.POCOProvider
{
	[Serializable]
	public class POCOItemUpdatedException : Exception
	{
		public POCOItemUpdatedException() { }

		public POCOItemUpdatedException(string message) : base(message) { }

		public POCOItemUpdatedException(string message, Exception inner) : base(message, inner) { }

		protected POCOItemUpdatedException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }
	}
}
