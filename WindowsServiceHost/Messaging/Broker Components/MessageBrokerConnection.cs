using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RabbitMQ.Client;

namespace DKK.Messaging
{
	public sealed class MessageBrokerConnection : IDisposable
	{
		public string Server { get; }
		public int Port { get; }
		public string VHost { get; }
		public IConnection Connection { get; private set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public MessageBrokerConnection(IEnumerable<KeyValuePair<string, string>> rabbitEnvironment)
		{
			this.Server = rabbitEnvironment.Single(kvp => kvp.Key == "RabbitMQServer").Value;
			this.Port = Convert.ToInt32(rabbitEnvironment.Single(kvp => kvp.Key == "RabbitMQPort").Value, CultureInfo.InvariantCulture);
			this.VHost = rabbitEnvironment.Single(kvp => kvp.Key == "RabbitMQVHost").Value;

			this.Connection = this.CreateConnection();
			this.Connection.ConnectionShutdown += Connection_ConnectionShutdown;
		}

		private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
		{
			(sender as IConnection).ConnectionShutdown -= Connection_ConnectionShutdown;
			this.Connection = null;
		}

		private IConnection CreateConnection()
		{
			var factory = new ConnectionFactory();
			factory.HostName = this.Server;
			factory.Port = this.Port;
			factory.VirtualHost = this.VHost;
			return factory.CreateConnection();
		}

		public void CloseConnection()
		{
			if (this.Connection!= null)
			{
				this.Connection.Dispose();
				this.Connection = null;
			}
		}

		#region IDisposable
		private bool disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					this.CloseConnection();
				}
			}

			disposed = true;
		}
		#endregion

		public void Close()
		{
			this.Dispose();
		}
	}
}
