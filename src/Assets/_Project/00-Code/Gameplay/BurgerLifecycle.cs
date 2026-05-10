using System.Collections.Generic;
using Mergeburgers.Data;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Gameplay
{
  /// <summary>
  /// Управляет жизнью бургеров на доске.
  /// После каждого хода: уменьшает LifeRemaining у всех бургеров,
  /// продаёт тех у кого LifeRemaining = 0 (фактически: бургер с life=0 после декремента
  /// убирается с доски, монеты уходят в Economy через сигнал).
  /// </summary>
  public sealed class BurgerLifecycle
  {
    private readonly RecipeDatabase _recipeDatabase;
    private readonly SignalBus _signalBus;

    [Inject]
    public BurgerLifecycle(RecipeDatabase recipeDatabase, SignalBus signalBus)
    {
      _recipeDatabase = recipeDatabase;
      _signalBus = signalBus;
    }

    /// <summary>
    /// Тик после каждого хода. Возвращает обновлённый state и Fire'ает сигналы продаж.
    /// </summary>
    public IngredientCell[,] TickAndSellExpired(IngredientCell[,] state)
    {
      int width = state.GetLength(0);
      int height = state.GetLength(1);
      var newState = (IngredientCell[,]) state.Clone();

      for (int x = 0; x < width; x++)
      for (int y = 0; y < height; y++)
      {
        var cell = newState[x, y];
        if (cell.IsEmpty || !cell.Type.IsBurger()) continue;

        var ticked = cell.WithLifeDecremented();

        if (ticked.LifeRemaining <= 0)
        {
          // Авто-продажа
          _signalBus.Fire(new BurgerSoldSignal(
            recipeId: cell.Type.ToString(),
            coins: cell.BurgerSellPrice,
            boardPosition: new Vector2Int(x, y)
          ));
          Debug.Log($"[Lifecycle] Auto-sold {cell.Type} at ({x},{y}) for {cell.BurgerSellPrice} coins");
          newState[x, y] = IngredientCell.Empty;
        }
        else
        {
          newState[x, y] = ticked;
        }
      }

      return newState;
    }
  }
}