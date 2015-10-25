using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Nack")]
	public sealed class StartComponentNack : CommandBase
	{
		public StartComponentNack()
		{
		}

		public StartComponentNack(ICommand source)
			:base(source)
		{
		}
	}
}
