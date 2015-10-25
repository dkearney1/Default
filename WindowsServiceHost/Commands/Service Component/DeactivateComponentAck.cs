using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ack")]
	public sealed class DeactivateComponentAck : CommandBase
	{
		public DeactivateComponentAck()
		{
		}

		public DeactivateComponentAck(ICommand source)
			: base(source)
		{
		}
	}
}
