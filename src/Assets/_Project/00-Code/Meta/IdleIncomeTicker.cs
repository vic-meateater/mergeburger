using System;
using Mergeburgers.Core;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Meta
{
  /// <summary>
  /// Idle-доход, считаемый по реальному времени (не Time.deltaTime).
  /// Корректно работает при паузах, сворачивании вкладки, переключении фокуса.
  /// </summary>
  public sealed class IdleIncomeTicker : MonoBehaviour
  {
    private SaveManager _saveManager;
    private EconomyManager _economy;
    private IdleIncomeCalculator _calculator;

    private long _lastTickUnix;

    [Inject]
    public void Construct(
      SaveManager saveManager,
      EconomyManager economy,
      IdleIncomeCalculator calculator)
    {
      _saveManager = saveManager;
      _economy = economy;
      _calculator = calculator;
    }

    private void Start()
    {
      ApplyOfflineEarnings();
      _lastTickUnix = NowUnix();
    }

    private void Update()
    {
      // Тикаем не чаще раза в секунду
      long now = NowUnix();
      long elapsed = now - _lastTickUnix;
      if (elapsed < 1) return;

      int rate = _calculator.CalculateRate(_saveManager.Current);
      if (rate > 0)
        _economy.AddBankCoins((int)(elapsed * rate));

      _lastTickUnix = now;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
      // При возврате фокуса — досчитываем доход за время пропуска.
      // Update сам это сделает на следующем кадре, но явная фиксация полезна
      // на тот случай если пауза длилась долго.
      if (hasFocus)
      {
        _lastTickUnix = NowUnix() - GetElapsedSinceLastTick();
      }
    }

    private long GetElapsedSinceLastTick()
    {
      return NowUnix() - _lastTickUnix;
    }

    private void ApplyOfflineEarnings()
    {
      var save = _saveManager.Current;
      if (save == null) return;

      long nowUnix = NowUnix();
      long elapsed = nowUnix - save.lastSessionEndTime;
      int earned = _calculator.CalculateOfflineEarnings(save, nowUnix);

      if (earned > 0)
      {
        //if (elapsed >= 60)
//          Debug.Log($"[Idle] Offline earnings (away {elapsed}s): +{earned}");
        //else
//          Debug.Log($"[Idle] Background earnings ({elapsed}s): +{earned}");

        _economy.AddBankCoins(earned);
      }

      save.lastSessionEndTime = nowUnix;
    }

    private void OnDestroy()
    {
      if (_saveManager?.Current != null)
        _saveManager.Current.lastSessionEndTime = NowUnix();
    }
    
    private static long NowUnix() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
  }
}