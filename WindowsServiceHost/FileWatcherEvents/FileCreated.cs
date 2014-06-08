using DKK.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.FileWatcherEvents
{
	[Serializable]
	public sealed class FileCreated : EventBase
	{
		protected override string routingKeyFormat
		{
			get { return @"FileWatcher.FileCreated.{0}.{1}"; }
		}

		protected override string routingKeyExplanation
		{
			get { return @"FileWatcher.FileCreated.Directory.Filename"; }
		}

		public string Directory { get; set; }
		public string Filename { get; set; }
		public string FullUNCPath { get; set; }
		public DateTimeOffset FileCreateDateTime { get; set; }

		public FileCreated()
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
