using DKK.ConsoleEventWatcher.SvcComponentConfig;
using DKK.Events;
using DKK.Messaging;
using DKK.ServiceHostEvents;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ConsoleEventWatcher
{
	class Program
	{
		static void Main(string[] args)
		{
			Dictionary<string, MessageBrokerConnection> messageBrokerConnections = new Dictionary<string, MessageBrokerConnection>();

			PopulateMessageBrokerConnections(messageBrokerConnections);
			Dictionary<string, IEventConsumer> eventConsumers = new Dictionary<string, IEventConsumer>();

			foreach (string environment in messageBrokerConnections.Keys)
			{
				Action<IBasicProperties, IEvent> consoleWriteLine = new Action<IBasicProperties, IEvent>((props, evnt) => Console.WriteLine("{0}: {1:G}->{2}", environment, evnt.Created.ToLocalTime(), evnt.RoutingKey));
				Action<IBasicProperties, IEvent> consoleWriteStatus = new Action<IBasicProperties, IEvent>((props, evnt) =>
				{
					ServiceComponentStatus scs = evnt as ServiceComponentStatus;
					if (scs != null)
						Console.WriteLine("{0}: Status: {1} for {2}ms, SubStatus: {3} for {4}ms", environment, scs.Status, scs.TimeInStatus.TotalMilliseconds, scs.SubStatus, scs.TimeInSubStatus.TotalMilliseconds);
					else
						Console.WriteLine("{0}: {1:G}->{2}", environment, evnt.Created.ToLocalTime(), evnt.RoutingKey);
				});

				IEventConsumer ec2 = new EventConsumer(messageBrokerConnections[environment].Connection);
				eventConsumers.Add(environment, ec2);

				#region Service Host events
				ec2.RegisterEventHandler<ServiceHostStateChange>(
					new ServiceHostStateChange() { Environment = "#", Machine = "#", Process = "#", State = "#" }.RoutingKey,
					consoleWriteLine
					);

				ec2.RegisterEventHandler<ServiceHostStateChangeFailed>(
					new ServiceHostStateChangeFailed() { Environment = "#", Machine = "#", Process = "#", State = "#" }.RoutingKey,
					consoleWriteLine
					); 
				#endregion

				#region Service Host Component events
				ec2.RegisterEventHandler<ServiceHostComponentStateChange>(
					new ServiceHostComponentStateChange() { Environment = "#", Machine = "#", Process = "#", ServiceComponent = "#", State = "#" }.RoutingKey,
					consoleWriteLine
					);

				ec2.RegisterEventHandler<ServiceHostComponentStateChangeFailed>(
					new ServiceHostComponentStateChangeFailed() { Environment = "#", Machine = "#", Process = "#", ServiceComponent = "#", State = "#" }.RoutingKey,
					consoleWriteLine
					); 
				#endregion

				#region Service Component events
				ec2.RegisterEventHandler<ServiceComponentStatus>(
					new ServiceComponentStatus() { Machine = "#", ServiceComponent = "#", Status = "#" }.RoutingKey,
					consoleWriteStatus
					);

				ec2.RegisterEventHandler<ServiceComponentStateChangeFailed>(
					new ServiceComponentStateChangeFailed() { Machine = "#", Process = "#", ServiceComponent = "#", Status = "#" }.RoutingKey,
					consoleWriteStatus
					);  
				#endregion
			}

			bool exit = false;
			Console.WriteLine("Press x to exit");
			do
			{
				ConsoleKeyInfo cki = Console.ReadKey(true);
				if (string.Compare("x", new string(cki.KeyChar, 1), true, CultureInfo.InvariantCulture) == 0)
					exit = true;
			}
			while (!exit);

			eventConsumers.Values.ToList().ForEach(ec => ec.Close());
			eventConsumers.Clear();
			messageBrokerConnections.Values.ToList().ForEach(mbc => mbc.Close());
			messageBrokerConnections.Clear();

			Console.WriteLine("Exiting.");
		}

		private static void PopulateMessageBrokerConnections(Dictionary<string, MessageBrokerConnection> messageBrokerConnections)
		{
			ISvcComponentConfig configClient = new SvcComponentConfigClient();
			IEnumerable<string> environments = configClient.GetEnvironments();
			foreach (string environment in environments)
			{
				List<KeyValuePair<string, string>> envConfig = configClient.GetEnvironmentConfig(environment);
				IEnumerable<KeyValuePair<string, string>> rabbitEnv = envConfig.Where(kvp => kvp.Key.StartsWith("RabbitMQ"));
				messageBrokerConnections.Add(environment, new MessageBrokerConnection(rabbitEnv));
			}
		}
	}
}
