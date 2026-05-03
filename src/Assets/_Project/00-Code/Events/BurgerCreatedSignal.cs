using UnityEngine;

namespace Mergeburgers.Events
{
  /// <summary>
  /// Срабатывает каждый раз, когда merge-резолвер собрал готовый рецепт.
  /// Слушатели: HUD (обновить счёт), AudioManager (звук), Telemetry, FX-spawner.
  /// </summary>
  public sealed class BurgerCreatedSignal
  {
    public string RecipeId { get; }
    public int Reward { get; }
    public Vector2Int BoardPosition { get; }

    public BurgerCreatedSignal(string recipeId, int reward, Vector2Int boardPosition)
    {
      RecipeId = recipeId;
      Reward = reward;
      BoardPosition = boardPosition;
    }
  }
}