using System;
using System.Collections.Generic;

namespace DKK.FileWatcherComponent
{
	[Serializable]
	public sealed class WatchedDirectoryInfo
	{
		string LocalPath { get; }
		bool IncludeSubdirectories { get; }
		TimeSpan PostCreationDelayBeforeSendingEvent { get; }
	}

	[Serializable]
	public sealed class FileWatcherParams
	{
		string RegistryFilename { get; }
		List<WatchedDirectoryInfo> WatchedDirectories { get; }
	}
}
