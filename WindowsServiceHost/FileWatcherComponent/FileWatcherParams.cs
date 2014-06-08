using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.FileWatcherComponent
{
	[Serializable]
	public sealed class WatchedDirectoryInfo
	{
		string LocalPath { get; set; }
		bool IncludeSubdirectories { get; set; }
		TimeSpan PostCreationDelayBeforeSendingEvent { get; set; }
	}

	[Serializable]
	public sealed class FileWatcherParams
	{
		string RegistryFilename { get; set; }
		List<WatchedDirectoryInfo> WatchedDirectories { get; set; }
	}
}
