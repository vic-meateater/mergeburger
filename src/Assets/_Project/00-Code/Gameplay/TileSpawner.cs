using System.Collections.Generic;
using Mergeburgers.Data;
using UnityEngine;

namespace Mergeburgers.Gameplay
{
  public sealed class TileSpawner
  {
    private readonly IngredientDatabase _database;

    public TileSpawner(IngredientDatabase database)
    {
      _database = database;
    }

    /// <summary>
    /// Спавнит 1 плитку на случайную пустую клетку.
    /// Возвращает позицию заспавненной, либо null если места нет.
    /// </summary>
    public Vector2Int? SpawnOne(IngredientType[,] state)
    {
      var emptyCells = FindEmptyCells(state);
      if (emptyCells.Count == 0) return null;

      var pos = emptyCells[Random.Range(0, emptyCells.Count)];
      var type = PickRandomBaseType();
      state[pos.x, pos.y] = type;
      return pos;
    }

    private List<Vector2Int> FindEmptyCells(IngredientType[,] state)
    {
      var result = new List<Vector2Int>();
      int w = state.GetLength(0);
      int h = state.GetLength(1);
      for (int x = 0; x < w; x++)
      for (int y = 0; y < h; y++)
        if (state[x, y] == IngredientType.None)
          result.Add(new Vector2Int(x, y));
      return result;
    }

    private IngredientType PickRandomBaseType()
    {
      // Преимущественно Bun/Patty, реже остальные. Балансим на Day 24.
      float r = Random.value;
      if (r < 0.30f) return IngredientType.Bun;
      if (r < 0.60f) return IngredientType.Patty;
      if (r < 0.72f) return IngredientType.Cheese;
      if (r < 0.84f) return IngredientType.Lettuce;
      if (r < 0.94f) return IngredientType.Tomato;
      return IngredientType.Sauce;
    }
  }
}