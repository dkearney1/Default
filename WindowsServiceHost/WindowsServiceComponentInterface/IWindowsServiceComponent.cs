using System;
using System.Collections.Generic;

namespace DKK.WindowsServiceComponentInterface
{
	public interface IWindowsServiceComponent : IDisposable
	{
		string ConfigURL { get; set; }
		string Configuration { get; set; }
		IEnumerable<KeyValuePair<string, string>> Environment { get; set; }

		void Start();
		void Stop();
		void Pause();
		void Continue();
	}
}
