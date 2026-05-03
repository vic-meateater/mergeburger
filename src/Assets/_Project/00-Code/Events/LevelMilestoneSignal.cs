namespace Mergeburgers.Events
{
  /// <summary>
  /// Достигнута ключевая веха в сессии (например, 10-й бургер подряд).
  /// Главный триггер для interstitial с cooldown — НЕ временной, событийный.
  /// Урок Quadraburgers: реклама привязана к действию игрока, не к таймеру.
  /// </summary>
  public sealed class LevelMilestoneSignal
  {
    public int MilestoneIndex { get; }

    public LevelMilestoneSignal(int milestoneIndex)
    {
      MilestoneIndex = milestoneIndex;
    }
  }
}