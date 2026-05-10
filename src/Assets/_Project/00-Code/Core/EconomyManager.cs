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

    public int CurrentCoins => _saveManager.Current?.coins ?? 0;

    private void OnBurgerSold(BurgerSoldSignal s)
    {
      if (_saveManager.Current == null) return;

      int oldValue = _saveManager.Current.coins;
      _saveManager.Current.coins += s.Coins;
      int newValue = _saveManager.Current.coins;

      Debug.Log($"[Economy] +{s.Coins} ({s.RecipeId}) → {newValue}");
      _signalBus.Fire(new CoinsChangedSignal(oldValue, newValue));
    }
  }
}