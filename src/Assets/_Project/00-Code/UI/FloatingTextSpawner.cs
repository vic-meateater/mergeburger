using Mergeburgers.Events;
using Mergeburgers.Gameplay;
using UnityEngine;
using Zenject;

namespace Mergeburgers.UI
{
  /// <summary>
  /// При продаже бургера спавнит FloatingText "+N" на позиции плитки.
  /// Висит на Canvas Game.
  /// </summary>
  public sealed class FloatingTextSpawner : MonoBehaviour
  {
    [SerializeField] private FloatingText _prefab;
    [SerializeField] private Transform _container; // обычно сам Canvas
    [SerializeField] private Board _board;
    [SerializeField] private Color _coinsColor = new Color(1f, 0.85f, 0.2f); // золотой

    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
      _signalBus = signalBus;
    }

    private void Start()
    {
      _signalBus.Subscribe<BurgerSoldSignal>(OnBurgerSold);
    }

    private void OnDestroy()
    {
      if (_signalBus != null)
        _signalBus.Unsubscribe<BurgerSoldSignal>(OnBurgerSold);
    }

    private void OnBurgerSold(BurgerSoldSignal s)
    {
      var tilePosition = _board.GetTileAnchoredPosition(s.BoardPosition);

      var floating = Instantiate(_prefab, _container);
      var rect = floating.GetComponent<RectTransform>();
      rect.anchoredPosition = tilePosition;
      floating.Play($"+{s.Coins}", _coinsColor);
    }
  }
}