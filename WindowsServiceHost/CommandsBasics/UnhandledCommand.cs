namespace DKK.Commands
{
	public sealed class UnhandledCommand : CommandBase
	{
		public UnhandledCommand()
			: base()
		{
		}

		public UnhandledCommand(ICommand src)
			: base(src)
		{
		}
	}
}
