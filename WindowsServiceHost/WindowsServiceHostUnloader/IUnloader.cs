using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WindowsServiceHostUnloader
{
	[ServiceContract(Namespace = "DKK.WindowsServiceHostUnloader.v1")]
	public interface IUnloader
	{
		[OperationContract]
		void Unload();
	}
}
