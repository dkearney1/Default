using DKK.Messaging;
using DKK.POCOProvider;
using DKK.POCOs;
using DKK.SampleServiceComponent;
using DKK.ServiceHostCommands;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkMaker
{
	class Program
	{
		static ServiceHost GetServiceHost(ServiceHostProvider provider)
		{
			ServiceHost sh = provider.Queryable().Single(q => q.Machine == Environment.MachineName);

			while (string.IsNullOrWhiteSpace(sh.CommandMessageQueue))
			{
				Thread.Sleep(500);
				sh = provider.Queryable().Single(q => q.Machine == Environment.MachineName);
			}

			return sh;
		}

		static void Main(string[] args)
		{
			TimeSpan timeout = TimeSpan.FromSeconds(1d);

			SvcComponentConfig.ISvcComponentConfig configClient = new SvcComponentConfig.SvcComponentConfigClient();

			// Get the list of environments, and make sure Dev is in the list
			IEnumerable<string> environments = configClient.GetEnvironments();
			string env = environments.Single(s => s == "Dev");

			// Get the Dev environment configuration
			IEnumerable<KeyValuePair<string, string>> environment = configClient.GetEnvironmentConfig(env);
			IEnumerable<KeyValuePair<string, string>> mongoEnv = environment.Where(kvp => kvp.Key.StartsWith("Mongo"));
			IEnumerable<KeyValuePair<string, string>> rabbitEnv = environment.Where(kvp => kvp.Key.StartsWith("Rabbit"));

			ServiceHostProvider provider = new ServiceHostProvider(mongoEnv);

			using (MessageBrokerConnection mbc = new MessageBrokerConnection(rabbitEnv))
			using (ICommandProducer cs = new CommandProducer(mbc.Connection))
			using (IWorkProducer wp = new WorkProducer(mbc.Connection))
			{
				bool exit = false;
				Console.WriteLine("Press F to reload Files");
				Console.WriteLine("Press C to reload Configuration");
				Console.WriteLine("Press T to sTart component");
				Console.WriteLine("Press P to stoP component");
				Console.WriteLine("Press A to Activate component");
				Console.WriteLine("Press D to Deactivate component");
				Console.WriteLine("Press S to pauSe component");
				Console.WriteLine("Press N to coNtinue component");
				Console.WriteLine("Press W to create work");
				Console.WriteLine("Press x to exit");

				do
				{
					ConsoleKeyInfo cki = Console.ReadKey(true);
					string keyChar = new string(cki.KeyChar, 1);

					if (string.Compare("x", keyChar, true, CultureInfo.InvariantCulture) == 0)
						exit = true;

					else if (string.Compare("w", keyChar, true, CultureInfo.InvariantCulture) == 0)
						wp.Publish(new SampleWorkItem(), "SampleWorkQueue");

					else
					{
						ServiceHost sh = GetServiceHost(provider);
						PublicationAddress pa = PublicationAddress.Parse(sh.CommandMessageQueue);

						if (string.Compare("f", keyChar, true, CultureInfo.InvariantCulture) == 0)
							cs.Publish(new ReloadFiles() { CorrelationId = Guid.NewGuid() }, pa, timeout);

						else if (string.Compare("c", keyChar, true, CultureInfo.InvariantCulture) == 0)
							cs.Publish(new ReloadConfiguration() { CorrelationId = Guid.NewGuid() }, pa, timeout);

						else if (string.Compare("t", keyChar, true, CultureInfo.InvariantCulture) == 0)
							cs.Publish(new StartComponent(sh.Components.First().Id) { CorrelationId = Guid.NewGuid() }, pa, timeout);

						else if (string.Compare("p", keyChar, true, CultureInfo.InvariantCulture) == 0)
							cs.Publish(new StopComponent(sh.Components.First().Id) { CorrelationId = Guid.NewGuid() }, pa, timeout);

						else if (string.Compare("a", keyChar, true, CultureInfo.InvariantCulture) == 0)
							cs.Publish(new ActivateComponent(sh.Components.First().Id) { CorrelationId = Guid.NewGuid() }, pa, timeout);

						else if (string.Compare("d", keyChar, true, CultureInfo.InvariantCulture) == 0)
							cs.Publish(new DeactivateComponent(sh.Components.First().Id) { CorrelationId = Guid.NewGuid() }, pa, timeout);

						else if (string.Compare("s", keyChar, true, CultureInfo.InvariantCulture) == 0)
							cs.Publish(new PauseComponent(sh.Components.First().Id) { CorrelationId = Guid.NewGuid() }, pa, timeout);

						else if (string.Compare("n", keyChar, true, CultureInfo.InvariantCulture) == 0)
							cs.Publish(new ContinueComponent(sh.Components.First().Id) { CorrelationId = Guid.NewGuid() }, pa, timeout);
					}
				}
				while (!exit);
			}
		}
	}
}
