using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ack")]
	public sealed class ActivateComponentAck : CommandBase
	{
		public ActivateComponentAck()
		{
		}

		public ActivateComponentAck(ICommand source)
			: base(source)
		{
		}
	}
}
