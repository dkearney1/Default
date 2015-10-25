using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DKK.Messaging
{
	public abstract class ChannelBase : IDisposable
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acks")]
		public event EventHandler<BasicAckEventArgs> BasicAcks;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Nacks")]
		public event EventHandler<BasicNackEventArgs> BasicNacks;
		public event EventHandler<EventArgs> BasicRecoverOk;
		public event EventHandler<BasicReturnEventArgs> BasicReturn;
		public event EventHandler<CallbackExceptionEventArgs> CallbackException;
		public event EventHandler<FlowControlEventArgs> FlowControl;
		public event EventHandler<ShutdownEventArgs> ModelShutdown;

		protected IConnection Connection { get; }
		protected IModel Channel { get; private set; }

		protected ChannelBase(IConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException(nameof(connection));

			this.Connection = connection;
			this.Channel = this.Connection.CreateModel();

			#region Hook up event handlers
			this.Channel.BasicAcks += this.BasicAckEventHandler;
			this.Channel.BasicNacks += this.BasicNackEventHandler;
			this.Channel.BasicRecoverOk += this.BasicRecoverOkEventHandler;
			this.Channel.BasicReturn += this.BasicReturnEventHandler;
			this.Channel.CallbackException += this.CallbackExceptionHandler;
			this.Channel.FlowControl += this.FlowControlEventHandler;
			this.Channel.ModelShutdown += this.ModelShutdownEventHandler;
			#endregion
		}

		#region IDisposable Members
		private bool disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.Channel.IsOpen)
						this.Channel.Close();

					this.Channel.BasicAcks -= this.BasicAckEventHandler;
					this.Channel.BasicNacks -= this.BasicNackEventHandler;
					this.Channel.BasicRecoverOk -= this.BasicRecoverOkEventHandler;
					this.Channel.BasicReturn -= this.BasicReturnEventHandler;
					this.Channel.CallbackException -= this.CallbackExceptionHandler;
					this.Channel.FlowControl -= this.FlowControlEventHandler;
					this.Channel.ModelShutdown -= this.ModelShutdownEventHandler;

					this.Channel.Dispose();
					this.Channel = null;
				}

				this.disposed = true;
			}
		}
		#endregion

		#region EventHandling
		private void BasicAckEventHandler(object sender, BasicAckEventArgs e)
		{
			this.BasicAcks?.Invoke(sender, e);
		}

		private void BasicNackEventHandler(object sender, BasicNackEventArgs e)
		{
			this.BasicNacks?.Invoke(sender, e);
		}

		private void BasicRecoverOkEventHandler(object sender, EventArgs e)
		{
			this.BasicRecoverOk?.Invoke(sender, e);
		}

		private void BasicReturnEventHandler(object sender, BasicReturnEventArgs e)
		{
			this.BasicReturn?.Invoke(sender, e);
		}

		private void CallbackExceptionHandler(object sender, CallbackExceptionEventArgs e)
		{
			this.CallbackException?.Invoke(sender, e);
		}

		private void FlowControlEventHandler(object sender, FlowControlEventArgs e)
		{
			this.FlowControl?.Invoke(sender, e);
		}

		private void ModelShutdownEventHandler(object sender, ShutdownEventArgs e)
		{
			this.ModelShutdown?.Invoke(sender, e);
		}
		#endregion
	}
}
