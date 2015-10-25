using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Nack")]
	public sealed class ReloadConfigurationNack : CommandBase
	{
		public ReloadConfigurationNack()
		{
		}

		public ReloadConfigurationNack(ICommand source)
			:base(source)
		{
		}
	}
}
