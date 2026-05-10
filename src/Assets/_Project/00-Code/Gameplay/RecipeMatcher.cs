using System.Collections.Generic;
using Mergeburgers.Data;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Gameplay
{
    /// <summary>
    /// Сканирует доску на совпадения с рецептами после слайда+мерджа.
    /// Учитывает уровни ингредиентов: множитель цены растёт со звёздами.
    /// Рецепты с MinLevelRequired не срабатывают, если в линии нет плитки нужного уровня.
    /// </summary>
    public sealed class RecipeMatcher
    {
        public sealed class Match
        {
            public RecipeData Recipe;
            public Vector2Int CenterPosition;
            public List<Vector2Int> ConsumedPositions;
            public int FinalPrice;     // base × multiplier
            public int Multiplier;
        }

        public sealed class MatchResult
        {
            public IngredientCell[,] NewState;
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

        public MatchResult FindAndApply(IngredientCell[,] state)
        {
            EnsureSorted();

            var newState = (IngredientCell[,])state.Clone();
            var matches = new List<Match>();
            int width = newState.GetLength(0);
            int height = newState.GetLength(1);

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

        private Match TryFindOne(IngredientCell[,] state, int width, int height)
        {
            foreach (var recipe in _sortedRecipes)
            {
                int len = recipe.Pattern.Count;

                // Горизонталь
                for (int y = 0; y < height; y++)
                    for (int startX = 0; startX <= width - len; startX++)
                        if (TryMatchAt(state, recipe, startX, y, dx: 1, dy: 0, out var match))
                            return match;

                // Вертикаль
                for (int x = 0; x < width; x++)
                    for (int startY = 0; startY <= height - len; startY++)
                        if (TryMatchAt(state, recipe, x, startY, dx: 0, dy: 1, out var match))
                            return match;
            }
            return null;
        }

        private bool TryMatchAt(IngredientCell[,] state, RecipeData recipe,
            int startX, int startY, int dx, int dy, out Match match)
        {
            int len = recipe.Pattern.Count;
            match = null;

            // Совпадение типов в одну сторону
            bool forward = true;
            for (int i = 0; i < len; i++)
            {
                if (state[startX + i * dx, startY + i * dy].Type != recipe.Pattern[i])
                {
                    forward = false;
                    break;
                }
            }

            // Совпадение в обратную (на симметричных не критично, но для несимметричных нужно)
            bool backward = true;
            for (int i = 0; i < len; i++)
            {
                if (state[startX + i * dx, startY + i * dy].Type != recipe.Pattern[len - 1 - i])
                {
                    backward = false;
                    break;
                }
            }

            if (!forward && !backward) return false;

            // Проверяем требование минимального уровня
            int maxLevelInLine = 0;
            int totalStars = 0;
            var positions = new List<Vector2Int>(len);
            for (int i = 0; i < len; i++)
            {
                var pos = new Vector2Int(startX + i * dx, startY + i * dy);
                positions.Add(pos);
                int lvl = state[pos.x, pos.y].Level;
                if (lvl > maxLevelInLine) maxLevelInLine = lvl;
                totalStars += lvl;
            }

            if (recipe.MinLevelRequired > 0 && maxLevelInLine < recipe.MinLevelRequired)
                return false;

            int multiplier = CalculateMultiplier(totalStars);

            match = new Match
            {
                Recipe = recipe,
                CenterPosition = new Vector2Int(startX + (len / 2) * dx, startY + (len / 2) * dy),
                ConsumedPositions = positions,
                FinalPrice = recipe.BaseSellPrice * multiplier,
                Multiplier = multiplier
            };
            return true;
        }

        /// <summary>
        /// Множитель цены по сумме звёзд в линии:
        ///   0 звёзд → ×1
        ///   1 звезда → ×2
        ///   2 звезды → ×3
        ///   3 звезды → ×4
        ///   и так далее: ×(stars + 1)
        /// </summary>
        private static int CalculateMultiplier(int totalStars)
        {
            return 1 + totalStars;
        }

        private void ApplyMatch(IngredientCell[,] state, Match match)
        {
            foreach (var pos in match.ConsumedPositions)
                state[pos.x, pos.y] = IngredientCell.Empty;

            // Бургер живёт 3 хода
            state[match.CenterPosition.x, match.CenterPosition.y]
                = IngredientCell.Burger(match.Recipe.Result, match.FinalPrice, lifeRemaining: 3);
        }

        private void EnsureSorted()
        {
            if (_sortedRecipes != null) return;

            _sortedRecipes = new List<RecipeData>(_database.All);
            // Длинные → короткие. King Burger проверяется первым,
            // чтобы 5-в-ряд не схлопнулось в Hamburger по сабпаттерну.
            _sortedRecipes.Sort((a, b) => b.Pattern.Count.CompareTo(a.Pattern.Count));
        }
    }
}