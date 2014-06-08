using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Messaging
{
	//public delegate void BasicAckEventHandler(object sender, BasicAckEventArgs e);
	//public delegate void BasicNackEventHandler(object sender, BasicNackEventArgs e);
	//public delegate void BasicRecoverOkEventHandler(object sender, EventArgs e);
	//public delegate void BasicReturnEventHandler(object sender, BasicReturnEventArgs e);
	//public delegate void CallbackExceptionEventHandler(object sender, CallbackExceptionEventArgs e);
	//public delegate void FlowControlEventHandler(object sender, FlowControlEventArgs e);
	//public delegate void ModelShutdownEventHandler(object sender, ShutdownEventArgs e);

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

		protected IConnection Connection { get; private set; }
		protected IModel Channel { get; private set; }

		//public event BasicAckEventHandler BasicAcks;
		//public event BasicNackEventHandler BasicNacks;
		//public event BasicRecoverOkEventHandler BasicRecoverOk;
		//public event BasicReturnEventHandler BasicReturn;
		//public event CallbackExceptionEventHandler CallbackException;
		//public event FlowControlEventHandler FlowControl;
		//public event ModelShutdownEventHandler ModelShutdown;

		protected ChannelBase(IConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException("connection");

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
				}

				this.disposed = true;
			}
		}
		#endregion

		#region EventHandling
		private void BasicAckEventHandler(object sender, BasicAckEventArgs e)
		{
			EventHandler<BasicAckEventArgs> handler = this.BasicAcks;
			if (handler != null)
				handler(sender, e);
		}

		private void BasicNackEventHandler(object sender, BasicNackEventArgs e)
		{
			EventHandler<BasicNackEventArgs> handler = this.BasicNacks;
			if (handler != null)
				handler(sender, e);
		}

		private void BasicRecoverOkEventHandler(object sender, EventArgs e)
		{
			EventHandler<EventArgs> handler = this.BasicRecoverOk;
			if (handler != null)
				handler(sender, e);
		}

		private void BasicReturnEventHandler(object sender, BasicReturnEventArgs e)
		{
			EventHandler<BasicReturnEventArgs> handler = this.BasicReturn;
			if (handler != null)
				handler(sender, e);
		}

		private void CallbackExceptionHandler(object sender, CallbackExceptionEventArgs e)
		{
			EventHandler<CallbackExceptionEventArgs> handler = this.CallbackException;
			if (handler != null)
				handler(sender, e);
		}

		private void FlowControlEventHandler(object sender, FlowControlEventArgs e)
		{
			EventHandler<FlowControlEventArgs> handler = this.FlowControl;
			if (handler != null)
				handler(sender, e);
		}

		private void ModelShutdownEventHandler(object sender, ShutdownEventArgs e)
		{
			EventHandler<ShutdownEventArgs> handler = this.ModelShutdown;
			if (handler != null)
				handler(sender, e);
		}
		#endregion
	}
}
