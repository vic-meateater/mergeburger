using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
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
      var movedContents = new List<RectTransform>();

      foreach (var op in ops)
      {
        var tile = grid[op.From.x, op.From.y];
        if (tile == null) continue;

        // Поднимаем плитку выше соседей, чтобы её содержимое ехало поверх их фонов
        tile.transform.SetAsLastSibling();

        // Двигаем только содержимое (иконка/буква/уровень), фон-ячейка остаётся на месте.
        // Смещение = разница позиций целевой и исходной клетки.
        var delta = targetPositions[op.To.x, op.To.y] - targetPositions[op.From.x, op.From.y];

        foreach (var content in tile.ContentTransforms)
        {
          content.anchoredPosition = Vector2.zero;
          sequence.Join(content.DOAnchorPos(delta, MoveDuration).SetEase(Ease.OutQuad));
          movedContents.Add(content);
        }
      }

      sequence.OnComplete(() =>
      {
        // Возвращаем содержимое в локальный ноль — дальше Board.RedrawGrid перерисует клетки
        foreach (var content in movedContents)
          content.anchoredPosition = Vector2.zero;
        tcs.TrySetResult(true);
      });
      return tcs.Task;
    }

    /// <summary>
    /// Анимация сборки бургера: scale-вспышка.
    /// </summary>
    public Task PlayBurgerCreated(Tile tile)
    {
      if (tile == null) return Task.CompletedTask;

      tile.transform.SetAsLastSibling();

      var tcs = new TaskCompletionSource<bool>();
      var sequence = DOTween.Sequence();
      var contents = tile.ContentTransforms;

      // Вспышка масштабом только содержимого (бургер), фон-ячейка не дёргается
      foreach (var content in contents)
      {
        content.localScale = Vector3.one;
        sequence.Join(DOTween.Sequence()
          .Append(content.DOScale(1.3f, MergePopDuration / 2).SetEase(Ease.OutQuad))
          .Append(content.DOScale(1.0f, MergePopDuration / 2).SetEase(Ease.InQuad)));
      }

      sequence.OnComplete(() =>
      {
        foreach (var content in contents)
          content.localScale = Vector3.one;
        tcs.TrySetResult(true);
      });

      return tcs.Task;
    }

    /// <summary>
    /// Анимация продажи: scale в 0 + fade.
    /// </summary>
    public Task PlayBurgerSold(Tile tile)
    {
      if (tile == null) return Task.CompletedTask;

      tile.transform.SetAsLastSibling();

      var tcs = new TaskCompletionSource<bool>();
      var sequence = DOTween.Sequence();
      var contents = tile.ContentTransforms;

      // Масштабируем/гасим только содержимое (бургер), фон-ячейка остаётся на месте
      foreach (var content in contents)
      {
        content.localScale = Vector3.one;
        sequence.Join(content.DOScale(0.0f, SellDuration).SetEase(Ease.InBack));

        var graphic = content.GetComponent<Graphic>();
        if (graphic != null)
          sequence.Join(graphic.DOFade(0f, SellDuration));
      }

      sequence.OnComplete(() =>
      {
        // Восстанавливаем для будущей перерисовки
        foreach (var content in contents)
        {
          content.localScale = Vector3.one;

          var graphic = content.GetComponent<Graphic>();
          if (graphic != null)
          {
            var color = graphic.color;
            color.a = 1f;
            graphic.color = color;
          }
        }
        tcs.TrySetResult(true);
      });

      return tcs.Task;
    }
  }
}