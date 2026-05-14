using System;
using Mergeburgers.Core;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Meta
{
  /// <summary>
  /// Сохраняет прогресс в облако по ключевым событиям, с throttle.
  /// Минимум 5 секунд между реальными SaveAsync — иначе Yandex throttling.
  /// При закрытии вкладки форсит сохранение через OnApplicationFocus(false).
  /// </summary>
  public sealed class AutosaveDispatcher : IInitializable, IDisposable
  {
    private const int ThrottleSeconds = 5;

    private readonly SignalBus _signalBus;
    private readonly SaveManager _saveManager;

    private long _lastSaveUnix;
    private bool _pendingSave;

    [Inject]
    public AutosaveDispatcher(SignalBus signalBus, SaveManager saveManager)
    {
      _signalBus = signalBus;
      _saveManager = saveManager;
    }

    public void Initialize()
    {
      _signalBus.Subscribe<GameOverSignal>(OnGameOver);
      _signalBus.Subscribe<CashedOutSignal>(OnCashedOut);
      // Можно добавить ещё: EnergyChangedSignal, UpgradePurchasedSignal (когда будет)
    }

    public void Dispose()
    {
      _signalBus.Unsubscribe<GameOverSignal>(OnGameOver);
      _signalBus.Unsubscribe<CashedOutSignal>(OnCashedOut);
    }

    private void OnGameOver(GameOverSignal s) => RequestSave("game_over");
    private void OnCashedOut(CashedOutSignal s) => RequestSave("cashed_out");

    /// <summary>
    /// Запрашивает сохранение. Если cooldown ещё не прошёл — отметит как pending,
    /// сохранится при следующей возможности.
    /// </summary>
    public void RequestSave(string reason)
    {
      long now = NowUnix();
      long elapsed = now - _lastSaveUnix;

      if (elapsed < ThrottleSeconds)
      {
        _pendingSave = true;
        Debug.Log($"[Autosave] Throttled ({reason}), pending");
        return;
      }

      _pendingSave = false;
      _lastSaveUnix = now;
      Debug.Log($"[Autosave] Save triggered by: {reason}");
      _ = _saveManager.SaveAsync();
    }

    /// <summary>
    /// Принудительное сохранение игнорируя throttle. Используется при закрытии вкладки.
    /// </summary>
    public void ForceSave(string reason)
    {
      _lastSaveUnix = NowUnix();
      _pendingSave = false;
      Debug.Log($"[Autosave] FORCE save: {reason}");
      _ = _saveManager.SaveAsync();
    }

    private static long NowUnix() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
  }
}