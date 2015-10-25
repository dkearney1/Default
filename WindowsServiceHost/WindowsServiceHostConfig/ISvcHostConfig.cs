using System.Collections.Generic;
using System.ServiceModel;

namespace DKK.WindowsServiceHostConfig
{
	[ServiceContract(Namespace="DKK.WindowsServiceHostConfig.v1")]
	public interface ISvcHostConfig
	{
		[OperationContract]
		IEnumerable<KeyValuePair<string, string>> GetConfig();
	}
}
