using System.Collections.Generic;
using Mergeburgers.Data;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Gameplay
{
  /// <summary>
  /// Чистая логика движения 2048-style. Не знает о Tile, transform, Unity.
  /// Получает на вход состояние доски, направление, возвращает новое состояние + операции.
  /// Тестируется юнит-тестами без движка (на Day 25 при желании).
  /// </summary>
  public sealed class MergeResolver
  {
    private readonly EvolutionChain _evolutionChain;

    [Inject]
    public MergeResolver(EvolutionChain evolutionChain)
    {
      _evolutionChain = evolutionChain;
    }

    public sealed class ResolveResult
    {
      public IngredientType[,] NewState;
      public List<MergeOperation> Operations;
      public bool AnyChange;
    }

    public ResolveResult Resolve(IngredientType[,] state, SwipeDirection direction)
    {
      int width = state.GetLength(0);
      int height = state.GetLength(1);

      var newState = (IngredientType[,])state.Clone();
      var ops = new List<MergeOperation>();
      bool anyChange = false;

      // Алгоритм 2048: для каждой "линии" (строки или столбца, в зависимости от направления)
      // 1) собираем не-None плитки в список,
      // 2) проходим по списку и сливаем соседние одинаковые,
      // 3) раскладываем результат к стенке.

      switch (direction)
      {
        case SwipeDirection.Right:
          for (int y = 0; y < height; y++)
            ProcessLine(newState, ops, y, width, height, isRow: true, toEnd: true);
          break;
        case SwipeDirection.Left:
          for (int y = 0; y < height; y++)
            ProcessLine(newState, ops, y, width, height, isRow: true, toEnd: false);
          break;
        case SwipeDirection.Up:
          for (int x = 0; x < width; x++)
            ProcessLine(newState, ops, x, width, height, isRow: false, toEnd: true);
          break;
        case SwipeDirection.Down:
          for (int x = 0; x < width; x++)
            ProcessLine(newState, ops, x, width, height, isRow: false, toEnd: false);
          break;
      }

      // Проверка: было ли реальное изменение
      for (int x = 0; x < width && !anyChange; x++)
      for (int y = 0; y < height && !anyChange; y++)
        if (state[x, y] != newState[x, y])
          anyChange = true;

      return new ResolveResult
      {
        NewState = newState,
        Operations = ops,
        AnyChange = anyChange
      };
    }

    /// <summary>
    /// Обрабатывает одну линию (строку или столбец) для движения в одну сторону.
    /// </summary>
    private void ProcessLine(IngredientType[,] state, List<MergeOperation> ops,
      int lineIndex, int width, int height, bool isRow, bool toEnd)
    {
      int length = isRow ? width : height;

      // Собираем не-None плитки с их исходными позициями
      var line = new List<(IngredientType type, Vector2Int from)>();
      for (int i = 0; i < length; i++)
      {
        var pos = isRow ? new Vector2Int(i, lineIndex) : new Vector2Int(lineIndex, i);
        if (state[pos.x, pos.y] != IngredientType.None)
          line.Add((state[pos.x, pos.y], pos));
      }

      // Если двигаемся к концу (right/up), обрабатываем с конца
      if (toEnd)
        line.Reverse();

      // Сливаем соседние одинаковые
      var merged = new List<(IngredientType type, Vector2Int from, Vector2Int? mergedFrom)>();
      int idx = 0;
      while (idx < line.Count)
      {
        if (idx + 1 < line.Count && line[idx].type == line[idx + 1].type)
        {
          var evolved = _evolutionChain.Evolve(line[idx].type);
          if (evolved != IngredientType.None)
          {
            merged.Add((evolved, line[idx].from, line[idx + 1].from));
            idx += 2;
            continue;
          }
        }

        merged.Add((line[idx].type, line[idx].from, null));
        idx++;
      }

      // Раскладываем merged обратно в state, начиная от стенки
      // Сначала очищаем линию
      for (int i = 0; i < length; i++)
      {
        var pos = isRow ? new Vector2Int(i, lineIndex) : new Vector2Int(lineIndex, i);
        state[pos.x, pos.y] = IngredientType.None;
      }

      // Расставляем
      for (int i = 0; i < merged.Count; i++)
      {
        int targetIdx = toEnd ? (length - 1 - i) : i;
        var targetPos = isRow ? new Vector2Int(targetIdx, lineIndex) : new Vector2Int(lineIndex, targetIdx);
        state[targetPos.x, targetPos.y] = merged[i].type;

        // Запись операций
        if (merged[i].mergedFrom.HasValue)
        {
          // Это merge — две плитки превратились в одну
          ops.Add(new MergeOperation(MergeOperationType.Merge, merged[i].from, targetPos, merged[i].type));
          ops.Add(new MergeOperation(MergeOperationType.Merge, merged[i].mergedFrom.Value, targetPos, merged[i].type));
        }
        else if (merged[i].from != targetPos)
        {
          // Это движение — плитка переехала
          ops.Add(new MergeOperation(MergeOperationType.Move, merged[i].from, targetPos, merged[i].type));
        }
      }
    }
  }
}