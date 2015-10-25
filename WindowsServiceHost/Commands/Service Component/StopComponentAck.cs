using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ack")]
	public sealed class StopComponentAck : CommandBase
	{
		public StopComponentAck()
		{
		}

		public StopComponentAck(ICommand source)
			: base(source)
		{
		}
	}
}
