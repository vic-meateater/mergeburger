namespace Mergeburgers.Events
{
  public sealed class CoinsChangedSignal
  {
    public int OldValue { get; }
    public int NewValue { get; }
    public int Delta => NewValue - OldValue;

    public CoinsChangedSignal(int oldValue, int newValue)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }
  }
}