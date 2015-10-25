using System;
using System.Runtime.InteropServices;

namespace DKK.WindowsServiceHost
{
	internal static class NativeMethods
	{
		[DllImport("ADVAPI32.DLL", EntryPoint = "SetServiceStatus")]
		internal static extern int SetServiceStatus(IntPtr hServiceStatus, ref ServiceHost.SERVICE_STATUS lpServiceStatus);
	}
}
