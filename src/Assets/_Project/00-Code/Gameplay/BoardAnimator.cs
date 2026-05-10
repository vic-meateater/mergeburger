using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Gameplay
{
  /// <summary>
  /// Проигрывает анимации движения, сборки и продажи плиток.
  /// Принимает на вход операции от MergeResolver и текущую сетку Tile-объектов.
  /// </summary>
  public sealed class BoardAnimator
  {
    private const float MoveDuration = 0.2f;
    private const float MergePopDuration = 0.15f;
    private const float SellDuration = 0.4f;

    /// <summary>
    /// Анимирует движение плиток по операциям. Возвращает когда все твины закончились.
    /// </summary>
    public Task PlayMoveOperations(List<MergeOperation> ops, Tile[,] grid, Vector2[,] targetPositions)
    {
      if (ops == null || ops.Count == 0)
        return Task.CompletedTask;

      var tcs = new TaskCompletionSource<bool>();
      var sequence = DOTween.Sequence();

      foreach (var op in ops)
      {
        var tile = grid[op.From.x, op.From.y];
        if (tile == null) continue;

        var rect = tile.GetComponent<RectTransform>();
        var targetPos = targetPositions[op.To.x, op.To.y];

        sequence.Join(rect.DOAnchorPos(targetPos, MoveDuration).SetEase(Ease.OutQuad));
      }

      sequence.OnComplete(() => tcs.TrySetResult(true));
      return tcs.Task;
    }

    /// <summary>
    /// Анимация сборки бургера: scale-вспышка.
    /// </summary>
    public Task PlayBurgerCreated(Tile tile)
    {
      if (tile == null) return Task.CompletedTask;

      var tcs = new TaskCompletionSource<bool>();
      var rect = tile.GetComponent<RectTransform>();

      rect.localScale = Vector3.one;
      DOTween.Sequence()
        .Append(rect.DOScale(1.3f, MergePopDuration / 2).SetEase(Ease.OutQuad))
        .Append(rect.DOScale(1.0f, MergePopDuration / 2).SetEase(Ease.InQuad))
        .OnComplete(() => tcs.TrySetResult(true));

      return tcs.Task;
    }

    /// <summary>
    /// Анимация продажи: scale в 0 + fade.
    /// </summary>
    public Task PlayBurgerSold(Tile tile)
    {
      if (tile == null) return Task.CompletedTask;

      var tcs = new TaskCompletionSource<bool>();
      var rect = tile.GetComponent<RectTransform>();
      var canvasGroup = tile.GetComponent<CanvasGroup>();
      if (canvasGroup == null)
        canvasGroup = tile.gameObject.AddComponent<CanvasGroup>();

      DOTween.Sequence()
        .Append(rect.DOScale(0.0f, SellDuration).SetEase(Ease.InBack))
        .Join(canvasGroup.DOFade(0f, SellDuration))
        .OnComplete(() =>
        {
          // Восстанавливаем для будущей перерисовки
          rect.localScale = Vector3.one;
          canvasGroup.alpha = 1f;
          tcs.TrySetResult(true);
        });

      return tcs.Task;
    }
  }
}