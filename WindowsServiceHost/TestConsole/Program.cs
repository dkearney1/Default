using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DKK.Commands;
using DKK.Messaging;
using DKK.POCOProvider;
using DKK.POCOs;
using DKK.SampleServiceComponent;
using DKK.ServiceHostCommands;
using Newtonsoft.Json;

namespace DKK.TestConsole
{
	class Program
	{
		static void Main2(string[] args)
		{
			var environment = new List<KeyValuePair<string, string>>();

			environment.Add(new KeyValuePair<string, string>("MongoServer", "localhost"));
			environment.Add(new KeyValuePair<string, string>("MongoPort", "27017"));
			environment.Add(new KeyValuePair<string, string>("MongoRepositorySvcConfig", "ServiceConfig"));
			environment.Add(new KeyValuePair<string, string>("RabbitMQServer", "localhost"));
			environment.Add(new KeyValuePair<string, string>("RabbitMQPort", "5672"));
			environment.Add(new KeyValuePair<string, string>("RabbitMQVHost", "/Dev"));

			var mongoEnv = environment.Where(kvp => kvp.Key.StartsWith("Mongo"));
			var rabbitEnv = environment.Where(kvp => kvp.Key.StartsWith("Rabbit"));

			using (var mbc = new MessageBrokerConnection(rabbitEnv))
			{
				using (var cl = new CommandConsumer(mbc.Connection))
				{
					cl.RegisterCommandHandler<ReloadFiles>((props, cmd) =>
					{
						//Console.WriteLine("{0:G}: {1}", cmd.Created.ToLocalTime(), props.Type);
						return new ReloadFilesAck(cmd);
					});

					var loops = 10000;
					var successCounter = 0;
					var failCounter = 0;
					var cumulativeTime = 0L;
					var sw = new Stopwatch();

					var cmdExchange = Constants.CmdExchangeSettings;
					using (var cs = new CommandProducer(mbc.Connection))
					{
						for (int i = 0; i < loops; i++)
						{
							sw.Reset();
							sw.Start();

							var reply = cs.Publish(new ReloadFiles(), cl.PublicationAddress, TimeSpan.FromMilliseconds(20d));
							sw.Stop();

							if (reply != null)
							{
								++successCounter;
								cumulativeTime += sw.ElapsedMilliseconds;
							}
							else
								++failCounter;
						}
					}

					Console.WriteLine("{0} Command(s) Sent, {1} Successes, {2} Fails, {3}ms elapsed", loops, successCounter, failCounter, cumulativeTime);
					Console.WriteLine("Press Enter to continue");
					Console.ReadLine();
				}
			}
		}

		static void Main(string[] args)
		{
			var mongoEnv = new List<KeyValuePair<string, string>>();

			mongoEnv.Add(new KeyValuePair<string, string>("MongoServer", "localhost"));
			mongoEnv.Add(new KeyValuePair<string, string>("MongoPort", "27017"));
			mongoEnv.Add(new KeyValuePair<string, string>("MongoRepositorySvcConfig", "ServiceConfig"));

			//ServiceComponentProvider scp = new ServiceComponentProvider(mongoEnv);
			//ServiceComponent sc = new ServiceComponent()
			//{
			//	Assembly = "SampleServiceComponent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
			//	Class = "DKK.SampleServiceComponent.SampleComponent",
			//	CommandMessageQueue = string.Empty,
			//	Config = string.Empty,
			//	CreateDate = DateTime.UtcNow,
			//	Creator = $@"{Environment.UserDomainName}\\{Environment.UserName}",
			//	FriendlyName = "Sample Component I",
			//	IsActive = true,
			//	IsPaused = false,
			//	Machine = Environment.MachineName,
			//	ParamsAssembly = "SampleServiceComponent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
			//	ParamsClass = "DKK.SampleServiceComponent.SampleComponentParams",
			//};

			//Guid origId = sc.Id;

			//scp.Insert(sc);

			//ServiceComponent sc2 = scp.Queryable().Single(x => x.Id == origId);

			//if (origId != sc2.Id)
			//	throw new Exception("Ids do not match");

			//ServiceComponent sc = scp.Queryable().FirstOrDefault();
			//SampleComponentParams theParams = new SampleComponentParams() { IdleMessageRate = 1 };

			//sc.Config = JsonConvert.SerializeObject(theParams, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
			//sc.ParamsAssembly = theParams.GetType().Assembly.FullName;
			//sc.ParamsClass = theParams.GetType().FullName;

			//scp.Update(sc);

			var theComponent = new SampleComponent();
			var theParams = new SampleComponentParams() { IdleMessageRate = 1, WorkQueue = "SampleWorkQueue" };

			var sc1 = new ServiceComponent()
			{
				Id = Guid.NewGuid(),
				Assembly = theComponent.GetType().Assembly.FullName,
				Class = theComponent.GetType().FullName,
				Config = JsonConvert.SerializeObject(theParams, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }),
				FriendlyName = "Sample Component I",
				IsActive = true,
				IsPaused = false,
				ParamsAssembly = theParams.GetType().Assembly.FullName,
				ParamsClass = theParams.GetType().FullName,
			};

			//ServiceComponent sc2 = new ServiceComponent()
			//{
			//	Id = Guid.NewGuid(),
			//	Assembly = theComponent.GetType().Assembly.FullName,
			//	Class = theComponent.GetType().FullName,
			//	Config = JsonConvert.SerializeObject(theParams, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }),
			//	FriendlyName = "Sample Component II",
			//	IsActive = true,
			//	IsPaused = false,
			//	ParamsAssembly = theParams.GetType().Assembly.FullName,
			//	ParamsClass = theParams.GetType().FullName,
			//};

			var sh = new ServiceHost()
			{
				Machine = Environment.MachineName,
			};
			sh.Components.AddRange(new ServiceComponent[] { sc1, /*sc2*/ });

			var shp = new ServiceHostProvider(mongoEnv);
			shp.Insert(sh);
		}
	}
}
