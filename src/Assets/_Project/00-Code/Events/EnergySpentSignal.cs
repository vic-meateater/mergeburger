namespace Mergeburgers.Events
{
  /// <summary>
  /// Игрок потратил жизнь (после game over). Слушатели: HUD, EnergyService.
  /// Восстановление энергии — отдельный EnergyRestoredSignal.
  /// </summary>
  public sealed class EnergySpentSignal
  {
    public int RemainingEnergy { get; }

    public EnergySpentSignal(int remainingEnergy)
    {
      RemainingEnergy = remainingEnergy;
    }
  }
}