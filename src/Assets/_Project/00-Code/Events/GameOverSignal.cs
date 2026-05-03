namespace Mergeburgers.Events
{
  /// <summary>
  /// Раунд завершён проигрышем (доска заполнилась без возможности слияния).
  /// Слушатели: UI (показать game over popup), Economy (записать highscore),
  /// AdService (триггер interstitial с шансом и cooldown).
  /// </summary>
  public sealed class GameOverSignal
  {
    public int FinalScore { get; }
    public int BurgersCreated { get; }

    public GameOverSignal(int finalScore, int burgersCreated)
    {
      FinalScore = finalScore;
      BurgersCreated = burgersCreated;
    }
  }
}