using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DKK.WindowsServiceHost
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				DebugTrace("Starting Service Host as an application.");

				using (ServiceHost serviceHost = new ServiceHost())
				{
					serviceHost.DebugStart();

                    var exit = false;
					Console.WriteLine("Press x to exit");
					do
					{
                        var cki = Console.ReadKey(true);
						if (string.Compare("x", new string(cki.KeyChar, 1), true, CultureInfo.InvariantCulture) == 0)
							exit = true;
					}
					while (!exit);

					serviceHost.DebugStop();
				}
			}
			else
			{
				DebugTrace("Starting Service Host as a service.");

				try
				{
                    var ServicesToRun = new ServiceBase[] { new ServiceHost() };
					ServiceBase.Run(ServicesToRun);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(string.Format("Caught {0}, \"{1}\"", ex.GetType().ToString(), ex.ToString()));
					DKKWindowsServiceHostEventSource.Log.Exception(ex);
				}
			}

			DebugTrace("Stopping Service Host.");
		}

		private static void DebugTrace(string message)
		{
			DKKWindowsServiceHostEventSource.Log.DebugTrace(message);
			Trace.WriteLine(message);
		}
	}
}
