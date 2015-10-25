using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ack")]
	public sealed class PauseComponentAck : CommandBase
	{
		public PauseComponentAck()
		{
		}

		public PauseComponentAck(ICommand source)
			: base(source)
		{
		}
	}
}
