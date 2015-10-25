using System;
using System.Diagnostics.Tracing;

namespace DKK.WindowsServiceHost
{
	[EventSource(Name = "DKK-WindowsServiceHost")]
	public sealed class DKKWindowsServiceHostEventSource : EventSource
	{
		public static DKKWindowsServiceHostEventSource Log = new DKKWindowsServiceHostEventSource();

		public class Keywords   // This is a bitvector
		{
			public const EventKeywords Debug = (EventKeywords)1;
			public const EventKeywords Default = (EventKeywords)2;
		}

		[Event(1, Keywords = Keywords.Debug, Level = EventLevel.Verbose)]
		public void DebugTrace(string Message) { WriteEvent(1, Message); }

		[Event(2, Keywords = Keywords.Default, Level = EventLevel.LogAlways)]
		public void ServiceStateChanging(string State) { WriteEvent(2, State); }

		[Event(3, Keywords = Keywords.Default, Level = EventLevel.Informational)]
		public void Activity(string Activity) { WriteEvent(3, Activity); }

		[Event(4, Keywords = Keywords.Debug, Level = EventLevel.Critical)]
		public void Exception(Exception Exception) { WriteEvent(4, Exception.ToString()); }
	}
}
