using System;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Core
{
  /// <summary>
  /// Подписан на все ключевые сигналы и пишет их в консоль.
  /// На время разработки — главный инструмент проверки, что события доходят.
  /// Перед релизом отключим биндинг.
  /// </summary>
  public sealed class DebugEventLogger : IInitializable, IDisposable
  {
    private readonly SignalBus _signalBus;

    public DebugEventLogger(SignalBus signalBus)
    {
      _signalBus = signalBus;
    }

    public void Initialize()
    {
      _signalBus.Subscribe<GameOverSignal>(OnGameOver);
      _signalBus.Subscribe<BurgerCreatedSignal>(OnBurgerCreated);
      _signalBus.Subscribe<EnergySpentSignal>(OnEnergySpent);
      _signalBus.Subscribe<LevelMilestoneSignal>(OnLevelMilestone);
    }

    public void Dispose()
    {
      _signalBus.Unsubscribe<GameOverSignal>(OnGameOver);
      _signalBus.Unsubscribe<BurgerCreatedSignal>(OnBurgerCreated);
      _signalBus.Unsubscribe<EnergySpentSignal>(OnEnergySpent);
      _signalBus.Unsubscribe<LevelMilestoneSignal>(OnLevelMilestone);
    }

    private void OnGameOver(GameOverSignal s) =>
      Debug.Log($"[Signals] GameOver: score={s.FinalScore}, burgers={s.BurgersCreated}");

    private void OnBurgerCreated(BurgerCreatedSignal s) =>
      Debug.Log($"[Signals] BurgerCreated: {s.RecipeId} +{s.Reward} @ {s.BoardPosition}");

    private void OnEnergySpent(EnergySpentSignal s) =>
      Debug.Log($"[Signals] EnergySpent: remaining={s.RemainingEnergy}");

    private void OnLevelMilestone(LevelMilestoneSignal s) =>
      Debug.Log($"[Signals] Milestone reached: #{s.MilestoneIndex}");
  }
}