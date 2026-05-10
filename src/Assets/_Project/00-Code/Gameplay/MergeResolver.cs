using System.Collections.Generic;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Gameplay
{
  public sealed class MergeResolver
  {
    private const int MaxLevel = 3;

    public sealed class ResolveResult
    {
      public IngredientCell[,] NewState;
      public List<MergeOperation> Operations;
      public bool AnyChange;
    }

    [Inject]
    public MergeResolver()
    {
    }

    public ResolveResult Resolve(IngredientCell[,] state, SwipeDirection direction)
    {
      int width = state.GetLength(0);
      int height = state.GetLength(1);
      var newState = (IngredientCell[,]) state.Clone();
      var ops = new List<MergeOperation>();

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

      bool anyChange = false;
      for (int x = 0; x < width && !anyChange; x++)
      for (int y = 0; y < height && !anyChange; y++)
      {
        var a = state[x, y];
        var b = newState[x, y];
        if (a.Type != b.Type || a.Level != b.Level)
          anyChange = true;
      }

      return new ResolveResult {NewState = newState, Operations = ops, AnyChange = anyChange};
    }

    private void ProcessLine(IngredientCell[,] state, List<MergeOperation> ops,
      int lineIndex, int width, int height, bool isRow, bool toEnd)
    {
      int length = isRow ? width : height;

      // Собираем не-пустые клетки + их исходные позиции
      var line = new List<(IngredientCell cell, Vector2Int from)>();
      for (int i = 0; i < length; i++)
      {
        var pos = isRow ? new Vector2Int(i, lineIndex) : new Vector2Int(lineIndex, i);
        if (!state[pos.x, pos.y].IsEmpty)
          line.Add((state[pos.x, pos.y], pos));
      }

      if (toEnd) line.Reverse();

      // Группируем и применяем правила, сохраняя источники для каждой результирующей плитки
      var merged = new List<(IngredientCell result, List<Vector2Int> sources)>();
      int idx = 0;
      while (idx < line.Count)
      {
        var firstCell = line[idx].cell;

        if (firstCell.Type.IsBurger())
        {
          merged.Add((firstCell, new List<Vector2Int> {line[idx].from}));
          idx++;
          continue;
        }

        int groupStart = idx;
        var groupType = firstCell.Type;

        while (idx < line.Count && line[idx].cell.Type == groupType)
          idx++;

        int groupSize = idx - groupStart;
        var groupSlice = line.GetRange(groupStart, groupSize);

        var resultCell = ApplyMergeRules(groupSlice);
        var sources = new List<Vector2Int>(groupSize);
        for (int j = 0; j < groupSize; j++) sources.Add(groupSlice[j].from);

        merged.Add((resultCell, sources));
      }

      // Очищаем линию
      for (int i = 0; i < length; i++)
      {
        var pos = isRow ? new Vector2Int(i, lineIndex) : new Vector2Int(lineIndex, i);
        state[pos.x, pos.y] = IngredientCell.Empty;
      }

      // Раскладываем merged к стене и записываем операции
      for (int i = 0; i < merged.Count; i++)
      {
        int targetIdx = toEnd ? (length - 1 - i) : i;
        var targetPos = isRow ? new Vector2Int(targetIdx, lineIndex) : new Vector2Int(lineIndex, targetIdx);
        state[targetPos.x, targetPos.y] = merged[i].result;

        var sources = merged[i].sources;

        if (sources.Count == 1)
        {
          // Просто движение
          if (sources[0] != targetPos)
            ops.Add(new MergeOperation(MergeOperationType.Move, sources[0], targetPos, merged[i].result.Type));
        }
        else
        {
          // Группа схлопнулась: все источники "уехали" в targetPos
          foreach (var src in sources)
            ops.Add(new MergeOperation(MergeOperationType.Merge, src, targetPos, merged[i].result.Type));
        }
      }
    }

    private static IngredientCell ApplyMergeRules(List<(IngredientCell cell, Vector2Int from)> group)
    {
      if (group.Count == 1) return group[0].cell;

      int firstLevel = group[0].cell.Level;
      bool allSameLevel = true;
      for (int i = 1; i < group.Count; i++)
        if (group[i].cell.Level != firstLevel)
        {
          allSameLevel = false;
          break;
        }

      if (allSameLevel)
      {
        if (group.Count >= 3 && firstLevel < MaxLevel)
          return IngredientCell.Of(group[0].cell.Type, firstLevel + 1);
        return IngredientCell.Of(group[0].cell.Type, firstLevel);
      }

      // Смешанные уровни: оставляем максимальный
      int maxLevel = firstLevel;
      for (int i = 1; i < group.Count; i++)
        if (group[i].cell.Level > maxLevel)
          maxLevel = group[i].cell.Level;

      return IngredientCell.Of(group[0].cell.Type, maxLevel);
    }
  }
}