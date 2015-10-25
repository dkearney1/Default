using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using DKK.Messaging;
using DKK.POCOProvider;
using DKK.POCOs;
using DKK.SampleServiceComponent;
using DKK.ServiceHostCommands;
using RabbitMQ.Client;

namespace WorkMaker
{
	class Program
	{
		static ServiceHost GetServiceHost(ServiceHostProvider provider)
		{
			var sh = provider.Queryable().Single(q => q.Machine == Environment.MachineName);

			while (string.IsNullOrWhiteSpace(sh.CommandMessageQueue))
			{
				Thread.Sleep(500);
				sh = provider.Queryable().Single(q => q.Machine == Environment.MachineName);
			}

			return sh;
		}

		static void Main(string[] args)
		{
			var timeout = TimeSpan.FromSeconds(1d);

			var configClient = new SvcComponentConfig.SvcComponentConfigClient();

			// Get the list of environments, and make sure Dev is in the list
			var environments = configClient.GetEnvironments();
			var env = environments.Single(s => s == "Dev");

			// Get the Dev environment configuration
			var environment = configClient.GetEnvironmentConfig(env);
			var mongoEnv = environment.Where(kvp => kvp.Key.StartsWith("Mongo"));
			var rabbitEnv = environment.Where(kvp => kvp.Key.StartsWith("Rabbit"));

			var provider = new ServiceHostProvider(mongoEnv);

			using (var mbc = new MessageBrokerConnection(rabbitEnv))
			using (var cs = new CommandProducer(mbc.Connection))
			using (var wp = new WorkProducer(mbc.Connection))
			{
				var exit = false;
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
					var cki = Console.ReadKey(true);
					string keyChar = new string(cki.KeyChar, 1);

					if (string.Compare("x", keyChar, true, CultureInfo.InvariantCulture) == 0)
						exit = true;

					else if (string.Compare("w", keyChar, true, CultureInfo.InvariantCulture) == 0)
						wp.Publish(new SampleWorkItem(), "SampleWorkQueue");

					else
					{
						var sh = GetServiceHost(provider);
						var pa = PublicationAddress.Parse(sh.CommandMessageQueue);

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
