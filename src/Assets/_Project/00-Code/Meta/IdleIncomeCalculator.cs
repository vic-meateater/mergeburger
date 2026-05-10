using System;
using Mergeburgers.Core;
using Mergeburgers.Gameplay;

namespace Mergeburgers.Meta
{
  public sealed class IdleIncomeCalculator
  {
    public int CalculateRate(GameSave save)
    {
      if (save?.unlockedRecipes == null) return 0;

      int totalRate = 0;
      foreach (var recipeId in save.unlockedRecipes)
      {
        if (Enum.TryParse<IngredientType>(recipeId, out var type) &&
            IdleIncomeRates.CoinsPerSecondByBurger.TryGetValue(type, out var rate))
        {
          totalRate += rate;
        }
      }
      return totalRate;
    }

    public int CalculateOfflineEarnings(GameSave save, long nowUnix)
    {
      if (save == null) return 0;

      long elapsedSeconds = nowUnix - save.lastSessionEndTime;
      if (elapsedSeconds <= 0) return 0;

      long capSeconds = IdleIncomeRates.MaxOfflineHours * 3600L;
      if (elapsedSeconds > capSeconds) elapsedSeconds = capSeconds;

      int rate = CalculateRate(save);
      return (int)(elapsedSeconds * rate);
    }
  }
}