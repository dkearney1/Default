using System.Collections.Generic;
using System.ServiceModel;

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
