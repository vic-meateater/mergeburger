using UnityEngine;

namespace Mergeburgers.Events
{
  /// <summary>
  /// Бургер продан (авто или вручную). Слушатели: Economy, Telemetry, FX.
  /// </summary>
  public sealed class BurgerSoldSignal
  {
    public string RecipeId { get; }
    public int Coins { get; }
    public Vector2Int BoardPosition { get; }

    public BurgerSoldSignal(string recipeId, int coins, Vector2Int boardPosition)
    {
      RecipeId = recipeId;
      Coins = coins;
      BoardPosition = boardPosition;
    }
  }
}