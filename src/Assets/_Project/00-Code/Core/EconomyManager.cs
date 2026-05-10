using System;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Core
{
  public sealed class EconomyManager : IInitializable, IDisposable
  {
    private readonly SignalBus _signalBus;
    private readonly SaveManager _saveManager;

    [Inject]
    public EconomyManager(SignalBus signalBus, SaveManager saveManager)
    {
      _signalBus = signalBus;
      _saveManager = saveManager;
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
      if (_saveManager.Current == null) return;

      _saveManager.Current.coins += s.Coins;
      Debug.Log($"[Economy] +{s.Coins} coins ({s.RecipeId}) → total {_saveManager.Current.coins}");

      // Не save на каждое событие — отложим до game over (Day 13.5 / 19)
      // Либо batched. На сегодня просто увеличиваем в памяти.
    }
  }
}