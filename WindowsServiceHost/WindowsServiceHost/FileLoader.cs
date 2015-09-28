using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DKK.WindowsServiceHost
{
	internal class FileLoader
	{
		private string FileUpdateLocation { get; set; }
		private string FileDestination { get; set; }
		private List<Regex> IgnoredFileSpecs { get; set; }

		public FileLoader(string fileUpdateLocation, string fileDestination, IEnumerable<string> ignoredFileSpecs)
		{
			this.FileUpdateLocation = fileUpdateLocation;
			this.FileDestination = fileDestination;
			this.IgnoredFileSpecs = (ignoredFileSpecs ?? new List<string>()).Select(fs => WildToRegex(fs)).ToList();
		}

		internal void Execute()
		{
			if (!Directory.Exists(this.FileUpdateLocation))
				throw new ArgumentException("Directory does not exist", "FileUpdateLocation");

            var prefix = this.FileUpdateLocation + @"\";
			foreach (var entry in Directory.EnumerateFileSystemEntries(this.FileUpdateLocation, "*.*", SearchOption.AllDirectories))
			{
                var isDir = Directory.Exists(entry);
                var currentEntry = Path.Combine(this.FileDestination, entry.Replace(prefix, string.Empty));

				if (isDir)
				{
                    var matchesIgnore = this.IgnoredFileSpecs.Any(ifs => ifs.IsMatch(currentEntry));

					if (!matchesIgnore)
					{
						if (!Directory.Exists(currentEntry))
						{
							DebugTrace(string.Format("Creating directory {0}", currentEntry));
							Directory.CreateDirectory(currentEntry);
						}
					}
					else if (Directory.Exists(currentEntry))
					{
						DebugTrace(string.Format("Deleting directory {0}", currentEntry));
						Directory.Delete(currentEntry, true);
					}
				}
				else
				{
                    var matchesIgnore = this.IgnoredFileSpecs.Any(ifs => ifs.IsMatch(currentEntry));

					if (!matchesIgnore)
					{
                        var srcFI = new FileInfo(entry);
                        var dstFI = new FileInfo(currentEntry);

						if (dstFI.Exists)
						{
							if (srcFI.LastWriteTimeUtc > dstFI.LastWriteTimeUtc)
							{
								DebugTrace(string.Format("Updating file {0}", currentEntry));
								File.Copy(entry, currentEntry, true);
							}
						}
						else
						{
							DebugTrace(string.Format("Creating file {0}", currentEntry));
							File.Copy(entry, currentEntry, true);
						}
					}
					else if (File.Exists(currentEntry))
					{
						DebugTrace(string.Format("Deleting file {0}", currentEntry));
						File.Delete(currentEntry);
					}
				}
			}
		}

		private Regex WildToRegex(string wild)
		{
			if (wild.StartsWith("*"))
				wild = '^' + wild;

			if (wild.EndsWith("*"))
				wild = wild + '$';

			return new Regex(wild.Replace(".", "\\.").Replace("*", ".*").Replace("?", "."), RegexOptions.Compiled | RegexOptions.IgnoreCase);
		}

		private void DebugTrace(string message)
		{
			DKKWindowsServiceHostEventSource.Log.DebugTrace(message);
			Trace.WriteLine(message);
		}
	}
}
