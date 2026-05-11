namespace Mergeburgers.Events
{
  public sealed class BankCoinsChangedSignal
  {
    public int OldValue { get; }
    public int NewValue { get; }
    public int Delta => NewValue - OldValue;

    public BankCoinsChangedSignal(int oldValue, int newValue)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }
  }
}