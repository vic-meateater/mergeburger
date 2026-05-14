using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Core
{
  /// <summary>
  /// Двухпотоковая экономика:
  ///   - sessionCoins: заработок текущей Game-сессии. Виден на Game HUD.
  ///   - coins: общая касса бургерной. Видна на MainMenu.
  /// При завершении сессии (Game Over или возврат в MainMenu) — CashOut переносит session → bank.
  /// </summary>
  public sealed class EconomyManager : IInitializable, System.IDisposable
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

    public int CurrentBank => _saveManager.Current?.coins ?? 0;
    public int CurrentSession => _saveManager.Current?.sessionCoins ?? 0;

    private void OnBurgerSold(BurgerSoldSignal s)
    {
      AddSessionCoins(s.Coins);
      Debug.Log($"[Economy] +{s.Coins} session ({s.RecipeId}) → session={CurrentSession}");
    }

    public void AddSessionCoins(int amount)
    {
      if (_saveManager.Current == null) return;
      int oldValue = _saveManager.Current.sessionCoins;
      _saveManager.Current.sessionCoins += amount;
      _signalBus.Fire(new SessionCoinsChangedSignal(oldValue, _saveManager.Current.sessionCoins));
    }

    public void AddBankCoins(int amount)
    {
      if (_saveManager.Current == null) return;
      int oldValue = _saveManager.Current.coins;
      _saveManager.Current.coins += amount;
      _signalBus.Fire(new BankCoinsChangedSignal(oldValue, _saveManager.Current.coins));
    }

    /// <summary>
    /// Переносит sessionCoins → bank coins. Вызывается при game over или возврате в меню.
    /// Возвращает сумму переноса (для popup'а).
    /// </summary>
    public int CashOut()
    {
      if (_saveManager.Current == null) return 0;

      int amount = _saveManager.Current.sessionCoins;
      if (amount <= 0) return 0;

      _saveManager.Current.sessionCoins = 0;
      _signalBus.Fire(new SessionCoinsChangedSignal(amount, 0));

      int oldBank = _saveManager.Current.coins;
      _saveManager.Current.coins += amount;
      _signalBus.Fire(new BankCoinsChangedSignal(oldBank, _saveManager.Current.coins));
      _signalBus.Fire(new CashedOutSignal(amount, _saveManager.Current.coins));


      Debug.Log($"[Economy] CashOut: +{amount} → bank={_saveManager.Current.coins}");
      return amount;
    }
  }
}