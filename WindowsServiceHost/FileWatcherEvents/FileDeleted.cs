using System;
using DKK.Events;

namespace DKK.FileWatcherEvents
{
	[Serializable]
	public sealed class FileDeleted : EventBase
	{
		protected override string routingKeyFormat => @"FileWatcher.FileDeleted.{0}.{1}";

		protected override string routingKeyExplanation => @"FileWatcher.FileDeleted.Directory.Filename";

		public string Directory { get; set; }
		public string Filename { get; set; }
		public string FullUNCPath { get; set; }
		public DateTimeOffset FileDeleteDateTime { get; set; }

		public FileDeleted()
			: base()
		{
		}

		public override string RoutingKey
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.Directory))
					throw new InvalidOperationException("'Directory' has not been set");
				if (string.IsNullOrWhiteSpace(this.Filename))
					throw new InvalidOperationException("'Filename' has not been set");

				return string.Format(this.routingKeyFormat, this.Directory, this.Filename);
			}
		}
	}
}
