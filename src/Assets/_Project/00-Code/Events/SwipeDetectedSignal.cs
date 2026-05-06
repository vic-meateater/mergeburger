namespace Mergeburgers.Events
{
  public enum SwipeDirection
  {
    None = 0,
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4,
  }

  /// <summary>
  /// Игрок сделал свайп. Слушатели: BoardController (двигает плитки в направлении).
  /// Игнорируется во время анимаций / показа рекламы / попапов
  /// (за фильтрацию отвечает подписчик, не источник).
  /// </summary>
  public sealed class SwipeDetectedSignal
  {
    public SwipeDirection Direction { get; }

    public SwipeDetectedSignal(SwipeDirection direction)
    {
      Direction = direction;
    }
  }
}