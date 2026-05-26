using System;
using Mergeburgers.Core;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Meta
{
  /// <summary>
  /// Управляет энергией игрока. Регенерация по реальному времени (unix seconds),
  /// корректна при паузах, сворачивании вкладки, оффлайне.
  /// </summary>
  public sealed class EnergyManager
  {
    private readonly SaveManager _saveManager;
    private readonly SignalBus _signalBus;

    [Inject]
    public EnergyManager(SaveManager saveManager, SignalBus signalBus)
    {
      _saveManager = saveManager;
      _signalBus = signalBus;
    }

    /// <summary>
    /// Возвращает актуальное значение энергии после пересчёта регенерации.
    /// Дёргать каждый раз когда нужно знать — внутри регенерация ленивая.
    /// </summary>
    public int Current
    {
      get
      {
        Regenerate();
        return _saveManager.Current?.energy ?? 0;
      }
    }

    public int Max => EnergyConfig.Max;

    public bool IsFull => Current >= Max;

    /// <summary>
    /// Сколько секунд до восстановления следующей единицы (если не полная).
    /// 0 если уже полная.
    /// </summary>
    public int SecondsUntilNext
    {
      get
      {
        if (IsFull) return 0;

        long now = NowUnix();
        long lastFull = _saveManager.Current.energyLastFullTime;
        long elapsedSinceLastFull = now - lastFull;
        long secondsIntoCurrentTick = elapsedSinceLastFull % EnergyConfig.RegenSecondsPerUnit;
        return (int) (EnergyConfig.RegenSecondsPerUnit - secondsIntoCurrentTick);
      }
    }

    /// <summary>
    /// Пытается потратить N энергии. Возвращает true если хватило.
    /// </summary>
    public bool TrySpend(int amount)
    {
      if (Current < amount) return false;

      int oldValue = _saveManager.Current.energy;
      _saveManager.Current.energy -= amount;

      // Если энергия была полной — фиксируем точку отсчёта регенерации СЕЙЧАС.
      // Это правильная семантика: "регенерация началась с момента траты".
      if (oldValue == Max)
        _saveManager.Current.energyLastFullTime = NowUnix();

      int newValue = _saveManager.Current.energy;
      _signalBus.Fire(new EnergyChangedSignal(oldValue, newValue));
      _signalBus.Fire(new EnergySpentSignal(newValue));

      return true;
    }

    /// <summary>
    /// Прибавляет N энергии (например, за rewarded ad). Не превышает Max.
    /// </summary>
    public void Add(int amount)
    {
      if (_saveManager.Current == null) return;

      int oldValue = _saveManager.Current.energy;
      int newValue = Mathf.Min(oldValue + amount, Max);
      _saveManager.Current.energy = newValue;

      // Если стало полной — обновляем energyLastFullTime
      if (newValue == Max)
        _saveManager.Current.energyLastFullTime = NowUnix();

      _signalBus.Fire(new EnergyChangedSignal(oldValue, newValue));
    }

    /// <summary>
    /// Пересчитывает энергию исходя из времени, прошедшего с energyLastFullTime.
    /// Лениво вызывается при чтении Current. Сама не Fire'ит сигнал — это делает caller.
    /// </summary>
    private void Regenerate()
    {
      var save = _saveManager.Current;
      if (save == null) return;
      if (save.energy >= Max) return;

      long now = NowUnix();
      long elapsed = now - save.energyLastFullTime;
      if (elapsed <= 0) return;

      int unitsRegenerated = (int) (elapsed / EnergyConfig.RegenSecondsPerUnit);
      if (unitsRegenerated <= 0) return;

      int oldValue = save.energy;
      int newValue = Mathf.Min(oldValue + unitsRegenerated, Max);
      save.energy = newValue;

      if (newValue == Max)
      {
        // Полная — фиксируем точку отсчёта на now, чтобы при следующей трате
        // регенерация начала считаться правильно
        save.energyLastFullTime = now;
      }
      else
      {
        // Не полная — сдвигаем energyLastFullTime на использованные тики
        save.energyLastFullTime += unitsRegenerated * EnergyConfig.RegenSecondsPerUnit;
      }

      if (newValue != oldValue)
        _signalBus.Fire(new EnergyChangedSignal(oldValue, newValue));
    }

    private static long NowUnix() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
  }
}