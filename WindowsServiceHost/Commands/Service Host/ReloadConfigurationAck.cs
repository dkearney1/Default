using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ack")]
	public sealed class ReloadConfigurationAck : CommandBase
	{
		public ReloadConfigurationAck()
		{
		}

		public ReloadConfigurationAck(ICommand source)
			: base(source)
		{
		}
	}
}
