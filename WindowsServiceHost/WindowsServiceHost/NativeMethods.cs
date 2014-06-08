using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DKK.WindowsServiceHost
{
	internal static class NativeMethods
	{
		[DllImport("ADVAPI32.DLL", EntryPoint = "SetServiceStatus")]
		internal static extern int SetServiceStatus(IntPtr hServiceStatus, ref ServiceHost.SERVICE_STATUS lpServiceStatus);
	}
}
