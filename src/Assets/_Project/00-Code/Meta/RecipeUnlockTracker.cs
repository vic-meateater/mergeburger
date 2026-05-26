using System;
using Mergeburgers.Core;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Meta
{
  public sealed class RecipeUnlockTracker : IInitializable, IDisposable
  {
    private readonly SignalBus _signalBus;
    private readonly SaveManager _saveManager;

    [Inject]
    public RecipeUnlockTracker(SignalBus signalBus, SaveManager saveManager)
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
      if (_saveManager.Current?.unlockedRecipes == null) return;
      if (_saveManager.Current.unlockedRecipes.Contains(s.RecipeId)) return;

      _saveManager.Current.unlockedRecipes.Add(s.RecipeId);
    }
  }
}