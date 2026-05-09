using System.Collections.Generic;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Gameplay
{
  /// <summary>
  /// Логика движения и схлопывания плиток (Day 12.5).
  /// Правила:
  ///   - 2 одинаковых соседа в линии → 1 (того же типа, уровень = max(а, б))
  ///   - 3+ одинаковых одного уровня в линии → 1 (того же типа, уровень+1, cap=3)
  ///   - 3+ смешанные уровни → группа считается по уровням отдельно
  /// </summary>
  public sealed class MergeResolver
  {
    private const int MaxLevel = 3;

    public sealed class ResolveResult
    {
      public IngredientCell[,] NewState;
      public bool AnyChange;
    }

    // [Inject]
    // public MergeResolver()
    // {
    // }

    public ResolveResult Resolve(IngredientCell[,] state, SwipeDirection direction)
    {
      int width = state.GetLength(0);
      int height = state.GetLength(1);
      var newState = (IngredientCell[,]) state.Clone();

      switch (direction)
      {
        case SwipeDirection.Right:
          for (int y = 0; y < height; y++)
            ProcessLine(newState, y, width, height, isRow: true, toEnd: true);
          break;
        case SwipeDirection.Left:
          for (int y = 0; y < height; y++)
            ProcessLine(newState, y, width, height, isRow: true, toEnd: false);
          break;
        case SwipeDirection.Up:
          for (int x = 0; x < width; x++)
            ProcessLine(newState, x, width, height, isRow: false, toEnd: true);
          break;
        case SwipeDirection.Down:
          for (int x = 0; x < width; x++)
            ProcessLine(newState, x, width, height, isRow: false, toEnd: false);
          break;
      }

      // Проверка: было ли реальное изменение
      bool anyChange = false;
      for (int x = 0; x < width && !anyChange; x++)
      for (int y = 0; y < height && !anyChange; y++)
      {
        var a = state[x, y];
        var b = newState[x, y];
        if (a.Type != b.Type || a.Level != b.Level)
          anyChange = true;
      }

      return new ResolveResult {NewState = newState, AnyChange = anyChange};
    }

    private void ProcessLine(IngredientCell[,] state, int lineIndex, int width, int height, bool isRow, bool toEnd)
    {
      int length = isRow ? width : height;

      // Собираем не-пустые клетки в порядке обработки
      var line = new List<IngredientCell>();
      for (int i = 0; i < length; i++)
      {
        var pos = isRow ? new Vector2Int(i, lineIndex) : new Vector2Int(lineIndex, i);
        if (!state[pos.x, pos.y].IsEmpty)
          line.Add(state[pos.x, pos.y]);
      }

      if (toEnd) line.Reverse();

      // Группируем подряд идущие одного типа (любого уровня) и применяем правила
      var merged = new List<IngredientCell>();
      int idx = 0;
      while (idx < line.Count)
      {
        int groupStart = idx;
        var groupType = line[idx].Type;

        // Расширяем группу: одинаковый тип, любой уровень
        while (idx < line.Count && line[idx].Type == groupType)
          idx++;

        int groupSize = idx - groupStart;
        var groupSlice = line.GetRange(groupStart, groupSize);

        // Применяем правило к группе
        ApplyMergeRules(groupSlice, merged);
      }

      // Очищаем линию
      for (int i = 0; i < length; i++)
      {
        var pos = isRow ? new Vector2Int(i, lineIndex) : new Vector2Int(lineIndex, i);
        state[pos.x, pos.y] = IngredientCell.Empty;
      }

      // Раскладываем merged к стене
      for (int i = 0; i < merged.Count; i++)
      {
        int targetIdx = toEnd ? (length - 1 - i) : i;
        var targetPos = isRow ? new Vector2Int(targetIdx, lineIndex) : new Vector2Int(lineIndex, targetIdx);
        state[targetPos.x, targetPos.y] = merged[i];
      }
    }

    /// <summary>
    /// Принимает связную группу одного типа (разных уровней).
    /// Применяет правила:
    ///   - 1 элемент: остаётся как есть
    ///   - 2+ всех одного уровня: 3+ → lvl+1, 2 → тот же уровень
    ///   - смесь уровней: разделяем по уровням, применяем то же правило к подгруппам
    /// </summary>
    private static void ApplyMergeRules(List<IngredientCell> group, List<IngredientCell> output)
    {
      if (group.Count == 0) return;

      if (group.Count == 1)
      {
        output.Add(group[0]);
        return;
      }

      // Все одного уровня?
      int firstLevel = group[0].Level;
      bool allSameLevel = true;
      for (int i = 1; i < group.Count; i++)
        if (group[i].Level != firstLevel)
        {
          allSameLevel = false;
          break;
        }

      if (allSameLevel)
      {
        if (group.Count >= 3 && firstLevel < MaxLevel)
        {
          // 3+ одного уровня → 1 уровня выше
          output.Add(IngredientCell.Of(group[0].Type, firstLevel + 1));
        }
        else
        {
          // 2 одного уровня, или 3+ на cap — всё в 1 того же уровня
          output.Add(IngredientCell.Of(group[0].Type, firstLevel));
        }

        return;
      }

      // Смешанные уровни в группе:
      // Правило: оставляем 1 плитку с максимальным уровнем в группе.
      // Это покрывает случай "Bun lvl 1 + Bun lvl 2 → Bun lvl 2" и
      // "Bun lvl 1 + Bun lvl 1 + Bun lvl 2 → Bun lvl 2" (младшие поглощены).
      int maxLevel = firstLevel;
      for (int i = 1; i < group.Count; i++)
        if (group[i].Level > maxLevel)
          maxLevel = group[i].Level;

      output.Add(IngredientCell.Of(group[0].Type, maxLevel));
    }
  }
}