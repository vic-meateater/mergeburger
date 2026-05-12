namespace Mergeburgers.Events
{
  public sealed class EnergyChangedSignal
  {
    public int OldValue { get; }
    public int NewValue { get; }

    public EnergyChangedSignal(int oldValue, int newValue)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }
  }
}