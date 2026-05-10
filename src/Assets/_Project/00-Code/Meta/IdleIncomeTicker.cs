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
    private SignalBus _signalBus;
    private SaveManager _saveManager;
    private IdleIncomeCalculator _calculator;

    private long _lastTickUnix;

    [Inject]
    public void Construct(
      SignalBus signalBus,
      SaveManager saveManager,
      IdleIncomeCalculator calculator)
    {
      _signalBus = signalBus;
      _saveManager = saveManager;
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
        AddCoins((int) (elapsed * rate));

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
      int earned = _calculator.CalculateOfflineEarnings(save, nowUnix);

      if (earned > 0)
      {
        Debug.Log($"[Idle] Offline earnings: +{earned}");
        AddCoins(earned);
      }

      save.lastSessionEndTime = nowUnix;
    }

    private void OnDestroy()
    {
      if (_saveManager?.Current != null)
        _saveManager.Current.lastSessionEndTime = NowUnix();
    }

    private void AddCoins(int amount)
    {
      if (_saveManager.Current == null) return;
      int oldValue = _saveManager.Current.coins;
      _saveManager.Current.coins += amount;
      int newValue = _saveManager.Current.coins;
      _signalBus.Fire(new CoinsChangedSignal(oldValue, newValue));
    }

    private static long NowUnix() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
  }
}