using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ack")]
	public sealed class ContinueComponentAck : CommandBase
	{
		public ContinueComponentAck()
		{
		}

		public ContinueComponentAck(ICommand source)
			: base(source)
		{
		}
	}
}
