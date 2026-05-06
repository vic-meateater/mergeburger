using Mergeburgers.Gameplay;
using UnityEngine;

namespace Mergeburgers.Data
{
    [CreateAssetMenu(fileName = "Ingredient_", menuName = "Mergeburgers/Ingredient", order = 1)]
    public sealed class IngredientData : ScriptableObject
    {
        [field: SerializeField] public IngredientType Type { get; private set; }
        [field: SerializeField] public string DisplayName  { get; private set; }
        [field: SerializeField] public Color PlaceholderColor { get; private set; }
        [field: SerializeField] public string PlaceholderLetter { get; private set; }
    }
}