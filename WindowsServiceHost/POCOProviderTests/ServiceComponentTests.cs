using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DKK.POCOProvider;
using DKK.POCOs;

namespace POCOProviderTests
{
	[TestClass]
	public class ServiceComponentTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			List<KeyValuePair<string, string>> mongoEnv = new List<KeyValuePair<string, string>>();

			mongoEnv.Add(new KeyValuePair<string, string>("MongoServer", "localhost"));
			mongoEnv.Add(new KeyValuePair<string, string>("MongoPort", "27017"));
			mongoEnv.Add(new KeyValuePair<string, string>("MongoRepositorySvcConfig", "ServiceConfig"));

			ServiceComponentProvider scp = new ServiceComponentProvider(mongoEnv);
			ServiceComponent sc = new ServiceComponent()
			{
				Assembly = "theAssembly",
				Class = "theClass",
				CommandMessageQueue = "theQueue",
				Config = "theConfig",
				CreateDate = DateTime.UtcNow,
				Creator = string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName),
				IsActive = false,
				IsPaused = false,
				Machine = Environment.MachineName,
				ParamsAssembly = "paramsAssembly",
				ParamsClass = "paramsClass"
			};

			Guid origId = sc.Id;
			
			scp.Insert(sc);

			ServiceComponent sc2 = scp.Queryable().Single(x => x.Id == origId);

			Assert.AreEqual<Guid>(origId, sc2.Id);

			scp.Delete(sc2);
		}
	}
}
