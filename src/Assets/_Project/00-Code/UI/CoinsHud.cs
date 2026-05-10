using DG.Tweening;
using Mergeburgers.Core;
using Mergeburgers.Events;
using TMPro;
using UnityEngine;
using Zenject;

namespace Mergeburgers.UI
{
  /// <summary>
  /// HUD с количеством монет в углу экрана.
  /// При изменении монет — плавно "прокатывает" значение через DOTween.
  /// </summary>
  public sealed class CoinsHud : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _label;
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
      // Стартовое значение из SaveManager (через EconomyManager)
      _displayedValue = _economy.CurrentCoins;
      UpdateLabel();

      _signalBus.Subscribe<CoinsChangedSignal>(OnCoinsChanged);
    }

    private void OnDestroy()
    {
      if (_signalBus != null)
        _signalBus.Unsubscribe<CoinsChangedSignal>(OnCoinsChanged);

      _activeTween?.Kill();
    }

    private void OnCoinsChanged(CoinsChangedSignal s)
    {
      _activeTween?.Kill();

      // Плавно прокатим _displayedValue от текущего к s.NewValue
      int from = _displayedValue;
      _activeTween = DOTween.To(
        () => _displayedValue,
        v => { _displayedValue = v; UpdateLabel(); },
        s.NewValue,
        _rollDuration
      ).SetEase(Ease.OutQuad);
    }

    private void UpdateLabel()
    {
      _label.text = _displayedValue.ToString();
    }
  }
}