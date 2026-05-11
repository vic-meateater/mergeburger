using DG.Tweening;
using Mergeburgers.Core;
using Mergeburgers.Events;
using TMPro;
using UnityEngine;
using Zenject;

namespace Mergeburgers.UI
{
  public enum CoinsHudMode
  {
    Bank,
    Session
  }

  public sealed class CoinsHud : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private CoinsHudMode _mode = CoinsHudMode.Bank;
    [SerializeField] private float _rollDuration = 0.5f;

    private SignalBus _signalBus;
    private EconomyManager _economy;

    private int _displayedValue;
    private Tween _activeTween;

    [Inject]
    public void Construct(SignalBus signalBus, EconomyManager economy)
    {
      _signalBus = signalBus;
      _economy = economy;
    }

    private void Start()
    {
      _displayedValue = _mode == CoinsHudMode.Bank ? _economy.CurrentBank : _economy.CurrentSession;
      UpdateLabel();

      if (_mode == CoinsHudMode.Bank)
        _signalBus.Subscribe<BankCoinsChangedSignal>(OnBankChanged);
      else
        _signalBus.Subscribe<SessionCoinsChangedSignal>(OnSessionChanged);
    }

    private void OnDestroy()
    {
      if (_signalBus == null) return;
      if (_mode == CoinsHudMode.Bank)
        _signalBus.Unsubscribe<BankCoinsChangedSignal>(OnBankChanged);
      else
        _signalBus.Unsubscribe<SessionCoinsChangedSignal>(OnSessionChanged);

      _activeTween?.Kill();
    }

    private void OnBankChanged(BankCoinsChangedSignal s) => RollTo(s.NewValue);
    private void OnSessionChanged(SessionCoinsChangedSignal s) => RollTo(s.NewValue);

    private void RollTo(int target)
    {
      _activeTween?.Kill();
      _activeTween = DOTween.To(
        () => _displayedValue,
        v =>
        {
          _displayedValue = v;
          UpdateLabel();
        },
        target,
        _rollDuration
      ).SetEase(Ease.OutQuad);
    }

    private void UpdateLabel()
    {
      _label.text = _displayedValue.ToString();
    }
  }
}