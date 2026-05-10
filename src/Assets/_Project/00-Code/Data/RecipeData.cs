using System.Collections.Generic;
using Mergeburgers.Gameplay;
using UnityEngine;

namespace Mergeburgers.Data
{
  /// <summary>
  /// Один рецепт: паттерн ингредиентов в линию → результат.
  /// Линия может быть горизонтальной или вертикальной, длиной 3-5.
  /// </summary>
  [CreateAssetMenu(fileName = "Recipe_", menuName = "Mergeburgers/Recipe", order = 3)]
  public sealed class RecipeData : ScriptableObject
  {
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public List<IngredientType> Pattern { get; private set; }
    [field: SerializeField] public IngredientType Result { get; private set; }
    [field: SerializeField] public int BaseSellPrice { get; private set; }

    [Tooltip("Минимальный уровень хотя бы одной плитки в линии. -1 = нет требования.")]
    [field: SerializeField] public int MinLevelRequired { get; private set; } = -1;
  }
}