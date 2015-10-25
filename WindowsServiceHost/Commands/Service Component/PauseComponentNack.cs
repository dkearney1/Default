using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Nack")]
	public sealed class PauseComponentNack : CommandBase
	{
		public PauseComponentNack()
		{
		}

		public PauseComponentNack(ICommand source)
			:base(source)
		{
		}
	}
}
