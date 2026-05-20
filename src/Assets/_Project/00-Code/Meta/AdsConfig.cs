namespace Mergeburgers.Meta
{
  public static class AdsConfig
  {
    // Каждый N-й проданный бургер триггерит проверку показа
    public const int InterstitialEveryNBurgers = 7;

    // Минимальный cooldown между нашими показами interstitial (секунды)
    public const int InterstitialCooldownSeconds = 120;

    // Placement IDs
    public const string InterstitialMilestonePlacement = "milestone_interstitial";
  }
}