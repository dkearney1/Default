using DKK.Commands;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Nack")]
	public sealed class ActivateComponentNack : CommandBase
	{
		public ActivateComponentNack()
		{
		}

		public ActivateComponentNack(ICommand source)
			:base(source)
		{
		}
	}
}
