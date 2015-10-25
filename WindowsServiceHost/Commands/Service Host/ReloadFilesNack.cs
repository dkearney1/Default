using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Nack")]
	public sealed class ReloadFilesNack : CommandBase
	{
		public ReloadFilesNack()
		{
		}

		public ReloadFilesNack(ICommand source)
			:base(source)
		{
		}
	}
}
