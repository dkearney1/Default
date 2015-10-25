using System;

namespace DKK.Messaging
{
	public interface IChannel : IDisposable
	{
		void Close();
	}
}
