namespace Mergeburgers.Events
{
  public sealed class CashedOutSignal
  {
    public int Amount { get; }
    public int NewBank { get; }

    public CashedOutSignal(int amount, int newBank)
    {
      Amount = amount;
      NewBank = newBank;
    }
  }
}