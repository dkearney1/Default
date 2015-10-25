using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ack")]
	public sealed class StartComponentAck : CommandBase
	{
		public StartComponentAck()
		{
		}

		public StartComponentAck(ICommand source)
			: base(source)
		{
		}
	}
}
