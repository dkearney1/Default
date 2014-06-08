using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DKK.WindowsServiceHostConfig
{
    [ServiceContract(Namespace="DKK.WindowsServiceHostConfig.v1")]
    public interface ISvcHostConfig
    {
        [OperationContract]
        IEnumerable<KeyValuePair<string, string>> GetConfig();
    }
}
