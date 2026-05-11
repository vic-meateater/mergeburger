namespace Mergeburgers.Events
{
  public sealed class SessionCoinsChangedSignal
  {
    public int OldValue { get; }
    public int NewValue { get; }
    public int Delta => NewValue - OldValue;

    public SessionCoinsChangedSignal(int oldValue, int newValue)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }
  }
}