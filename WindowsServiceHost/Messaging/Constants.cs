using System;
using System.Globalization;
using System.IO;
using RabbitMQ.Client;

namespace DKK.Messaging
{
	public sealed class Constants
	{
		/// <summary>
		/// Private constructor prevents compiler from adding a default public constructor to a static class
		/// </summary>
		private Constants() { }

		public const byte NonPersistent = 1;
		public const byte Persistent = 2;

		public const string DefaultExchangeName = "";
		public const string EventExchangeName = "EventExchange";
		public const string WorkExchangeName = "WorkExchange";
		public const string CmdExchangeName = "CmdExchange";
		public static string PrivateQueueName => string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}", Environment.MachineName, Path.GetFileNameWithoutExtension(Environment.CommandLine.Replace("\"", string.Empty)), Guid.NewGuid().ToString("N").Substring(0, 8));

		public static ExchangeSettings DefaultExchangeSettings => new ExchangeSettings()
		{
			Name = Constants.DefaultExchangeName,
			ExchangeType = ExchangeType.Direct,
			Durable = true,
			AutoDelete = false,
			Arguments = null
		};

		public static ExchangeSettings EventExchangeSettings => new ExchangeSettings()
		{
			Name = Constants.EventExchangeName,
			ExchangeType = ExchangeType.Topic,
			Durable = true,
			AutoDelete = false,
			Arguments = null
		};

		public static ExchangeSettings CmdExchangeSettings => new ExchangeSettings()
		{
			Name = Constants.CmdExchangeName,
			ExchangeType = ExchangeType.Direct,
			Durable = true,
			AutoDelete = false,
			Arguments = null
		};

		public static ExchangeSettings WorkExchangeSettings => new ExchangeSettings()
		{
			Name = Constants.WorkExchangeName,
			ExchangeType = ExchangeType.Direct,
			Durable = true,
			AutoDelete = false,
			Arguments = null
		};

		public static QueueSettings PrivateQueueSettings => new QueueSettings()
		{
			Name = Constants.PrivateQueueName,
			Durable = false,
			AutoDelete = false,
			Exclusive = true,
			Arguments = null
		};

		public static QueueSettings WorkQueueSettings => new QueueSettings()
		{
			Name = string.Empty,
			Durable = true,
			AutoDelete = false,
			Exclusive = false,
			Arguments = null
		};
	}
}
