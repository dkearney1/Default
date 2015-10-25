using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ack")]
	public sealed class ReloadFilesAck : CommandBase
	{
		public ReloadFilesAck()
		{
		}

		public ReloadFilesAck(ICommand source)
			: base(source)
		{
		}
	}
}
