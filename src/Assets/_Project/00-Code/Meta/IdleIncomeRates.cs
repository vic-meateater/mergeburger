using System.Collections.Generic;
using Mergeburgers.Gameplay;

namespace Mergeburgers.Meta
{
  /// <summary>
  /// Таблица пассивного дохода по типам разблокированных бургеров.
  /// На Day 24 балансим, на Day 18 вытащим в ScriptableObject если будет много вариантов.
  /// </summary>
  public static class IdleIncomeRates
  {
    public static readonly Dictionary<IngredientType, int> CoinsPerSecondByBurger = new()
    {
      { IngredientType.Hamburger, 1 },
      { IngredientType.Cheeseburger, 3 },
      { IngredientType.Veggieburger, 3 },
      { IngredientType.BigMac, 10 },
      { IngredientType.KingBurger, 50 }
    };

    public const int MaxOfflineHours = 8;
  }
}