using Mergeburgers.Events;

namespace Mergeburgers.Gameplay
{
  public sealed class GameOverChecker
  {
    private readonly MergeResolver _resolver;

    public GameOverChecker(MergeResolver resolver)
    {
      _resolver = resolver;
    }

    /// <summary>
    /// Проверяет: если любой из 4 свайпов изменит доску, значит ходы есть.
    /// Использует MergeResolver на копии состояния — оригинал не трогает.
    /// </summary>
    public bool IsGameOver(IngredientCell[,] state)
    {
      // Если есть пустые клетки — точно есть валидный ход (двигай в их сторону)
      int w = state.GetLength(0);
      int h = state.GetLength(1);
      for (int x = 0; x < w; x++)
      for (int y = 0; y < h; y++)
        if (state[x, y].IsEmpty)
          return false;

      // Доска полна — проверяем все 4 направления
      var directions = new[]
      {
        SwipeDirection.Up, SwipeDirection.Down,
        SwipeDirection.Left, SwipeDirection.Right
      };

      foreach (var dir in directions)
      {
        var stateCopy = (IngredientCell[,]) state.Clone();
        var result = _resolver.Resolve(stateCopy, dir);
        if (result.AnyChange) return false;
      }

      return true;
    }
  }
}