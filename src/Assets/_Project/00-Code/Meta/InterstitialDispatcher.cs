using System;
using Mergeburgers.Events;
using Mergeburgers.Yandex;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Meta
{
  /// <summary>
  /// Решает когда показать interstitial. Подписан на BurgerSoldSignal.
  /// Срабатывает каждый N-й бургер, но с cooldown в M секунд.
  /// Главный урок Квадрабургеров: реклама = по событиям, не по таймеру.
  /// </summary>
  public sealed class InterstitialDispatcher : IInitializable, IDisposable
  {
    private readonly SignalBus _signalBus;
    private readonly IAdService _adService;

    private int _burgersSoldCounter;
    private long _lastShownUnix;

    [Inject]
    public InterstitialDispatcher(SignalBus signalBus, IAdService adService)
    {
      _signalBus = signalBus;
      _adService = adService;
    }

    public void Initialize()
    {
      _signalBus.Subscribe<BurgerSoldSignal>(OnBurgerSold);
    }

    public void Dispose()
    {
      _signalBus.Unsubscribe<BurgerSoldSignal>(OnBurgerSold);
    }

    private void OnBurgerSold(BurgerSoldSignal s)
    {
      _burgersSoldCounter++;

      if (_burgersSoldCounter < AdsConfig.InterstitialEveryNBurgers)
        return;

      // Достигли порога — но проверяем cooldown
      long now = NowUnix();
      long elapsed = now - _lastShownUnix;

      if (elapsed < AdsConfig.InterstitialCooldownSeconds)
      {
        Debug.Log($"[Ads] Skip interstitial (cooldown {elapsed}s < {AdsConfig.InterstitialCooldownSeconds}s)");
        // Не сбрасываем counter — игрок не виноват, попробуем на следующем бургере
        return;
      }

      // Показываем — сбрасываем counter и фиксируем время
      _burgersSoldCounter = 0;
      _lastShownUnix = now;

      Debug.Log($"[Ads] Triggering interstitial after {AdsConfig.InterstitialEveryNBurgers} burgers");
      _ = ShowAsync();
    }

    private async System.Threading.Tasks.Task ShowAsync()
    {
      try
      {
        await _adService.ShowInterstitialAsync(AdsConfig.InterstitialMilestonePlacement);
        Debug.Log("[Ads] Interstitial completed");
      }
      catch (Exception e)
      {
        Debug.LogError($"[Ads] Interstitial failed: {e.Message}");
      }
    }

    private static long NowUnix() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
  }
}