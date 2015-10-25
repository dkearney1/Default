using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Nack")]
	public sealed class StopComponentNack : CommandBase
	{
		public StopComponentNack()
		{
		}

		public StopComponentNack(ICommand source)
			:base(source)
		{
		}
	}
}
