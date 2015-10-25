namespace DKK.Events
{
	public interface IQueueChangeEvent : IEvent
	{
		string FromQueue { get; }
		string ToQueue { get; }
		string Type { get; }
	}
}
