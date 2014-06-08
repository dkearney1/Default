using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DKK.WindowsServiceComponentConfig
{
	[ServiceContract(Namespace = "DKK.WindowsServiceComponentConfig.v1")]
	public interface ISvcComponentConfig
	{
		[OperationContract]
		IEnumerable<string> GetEnvironments();

		[OperationContract]
		IEnumerable<KeyValuePair<string, string>> GetEnvironmentConfig(string environment);
	}
}
