using System.Collections.Generic;
using Mergeburgers.Data;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Gameplay
{
  /// <summary>
  /// После эволюций сканирует доску и применяет рецепты.
  /// Линии ищутся горизонтально и вертикально, сравниваются с паттернами рецептов.
  /// Длиннее рецепты проверяются ПЕРЕД короткими (BigMac до Hamburger),
  /// чтобы 5-ингредиентная линия не схлопнулась в Hamburger по сабпаттерну.
  /// </summary>
  public sealed class RecipeMatcher
  {
    public sealed class Match
    {
      public RecipeData Recipe;
      public Vector2Int CenterPosition;
      public List<Vector2Int> ConsumedPositions;
    }

    public sealed class MatchResult
    {
      public IngredientType[,] NewState;
      public List<Match> Matches;
      public bool AnyMatch;
    }

    private readonly RecipeDatabase _database;
    private List<RecipeData> _sortedRecipes;

    [Inject]
    public RecipeMatcher(RecipeDatabase database)
    {
      _database = database;
    }

    public MatchResult FindAndApply(IngredientType[,] state)
    {
      EnsureSorted();

      var newState = (IngredientType[,]) state.Clone();
      var matches = new List<Match>();
      int width = newState.GetLength(0);
      int height = newState.GetLength(1);

      // Многопроходный поиск: после применения рецепта проверяем заново,
      // т.к. могли образоваться новые линии. Лимит — 8 проходов на всякий случай.
      for (int pass = 0; pass < 8; pass++)
      {
        var found = TryFindOne(newState, width, height);
        if (found == null) break;
        ApplyMatch(newState, found);
        matches.Add(found);
      }

      return new MatchResult
      {
        NewState = newState,
        Matches = matches,
        AnyMatch = matches.Count > 0
      };
    }

    private Match TryFindOne(IngredientType[,] state, int width, int height)
    {
      // Перебираем все рецепты от длинных к коротким
      foreach (var recipe in _sortedRecipes)
      {
        int len = recipe.Pattern.Count;

        // Горизонтальные линии
        for (int y = 0; y < height; y++)
        for (int startX = 0; startX <= width - len; startX++)
        {
          if (MatchesAt(state, recipe, startX, y, dx: 1, dy: 0))
          {
            return new Match
            {
              Recipe = recipe,
              CenterPosition = new Vector2Int(startX + len / 2, y),
              ConsumedPositions = CollectPositions(startX, y, dx: 1, dy: 0, len)
            };
          }
        }

        // Вертикальные линии
        for (int x = 0; x < width; x++)
        for (int startY = 0; startY <= height - len; startY++)
        {
          if (MatchesAt(state, recipe, x, startY, dx: 0, dy: 1))
          {
            return new Match
            {
              Recipe = recipe,
              CenterPosition = new Vector2Int(x, startY + len / 2),
              ConsumedPositions = CollectPositions(x, startY, dx: 0, dy: 1, len)
            };
          }
        }
      }

      return null;
    }

    private bool MatchesAt(IngredientType[,] state, RecipeData recipe, int startX, int startY, int dx, int dy)
    {
      int len = recipe.Pattern.Count;

      // Прямое совпадение
      bool forward = true;
      for (int i = 0; i < len; i++)
      {
        if (state[startX + i * dx, startY + i * dy] != recipe.Pattern[i])
        {
          forward = false;
          break;
        }
      }

      if (forward) return true;

      // Обратное (для симметричных типа Bun-Patty-Bun неважно, но для несимметричных нужна вторая попытка)
      bool backward = true;
      for (int i = 0; i < len; i++)
      {
        if (state[startX + i * dx, startY + i * dy] != recipe.Pattern[len - 1 - i])
        {
          backward = false;
          break;
        }
      }

      return backward;
    }

    private static List<Vector2Int> CollectPositions(int startX, int startY, int dx, int dy, int len)
    {
      var list = new List<Vector2Int>(len);
      for (int i = 0; i < len; i++)
        list.Add(new Vector2Int(startX + i * dx, startY + i * dy));
      return list;
    }

    private void ApplyMatch(IngredientType[,] state, Match match)
    {
      // Очищаем все позиции линии
      foreach (var pos in match.ConsumedPositions)
        state[pos.x, pos.y] = IngredientType.None;

      // Кладём результат в центр
      state[match.CenterPosition.x, match.CenterPosition.y] = match.Recipe.Result;
    }

    private void EnsureSorted()
    {
      if (_sortedRecipes != null) return;

      _sortedRecipes = new List<RecipeData>(_database.All);
      // Длинные рецепты раньше коротких
      _sortedRecipes.Sort((a, b) => b.Pattern.Count.CompareTo(a.Pattern.Count));
    }
  }
}