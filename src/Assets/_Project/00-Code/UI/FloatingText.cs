using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Mergeburgers.UI
{
  public sealed class FloatingText : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _floatDistance = 120f;
    [SerializeField] private float _duration = 1.2f;

    public void Play(string text, Color color)
    {
      _label.text = text;
      _label.color = color;
      _canvasGroup.alpha = 1f;

      var rect = GetComponent<RectTransform>();
      var startPos = rect.anchoredPosition;
      var targetPos = startPos + Vector2.up * _floatDistance;

      DOTween.Sequence()
        .Append(rect.DOAnchorPos(targetPos, _duration).SetEase(Ease.OutQuad))
        .Join(_canvasGroup.DOFade(0f, _duration).SetEase(Ease.InQuad))
        .OnComplete(() => Destroy(gameObject));
    }
  }
}