using System.Collections.Generic;
using Mergeburgers.Gameplay;
using UnityEngine;

namespace Mergeburgers.Data
{
    /// <summary>
    /// Реестр всех ингредиентов. Один экземпляр на проект, инжектится через Zenject.
    /// </summary>
    [CreateAssetMenu(fileName = "IngredientDatabase", menuName = "Mergeburgers/Ingredient Database", order = 0)]
    public sealed class IngredientDatabase : ScriptableObject
    {
        [SerializeField] private List<IngredientData> _ingredients;

        private Dictionary<IngredientType, IngredientData> _lookup;

        public IngredientData Get(IngredientType type)
        {
            if (_lookup == null)
                BuildLookup();

            return _lookup != null && _lookup.TryGetValue(type, out var data) ? data : null;
        }

        public IReadOnlyList<IngredientData> All => _ingredients;

        private void BuildLookup()
        {
            _lookup = new Dictionary<IngredientType, IngredientData>(_ingredients.Count);
            foreach (var ingredient in _ingredients)
            {
                if (ingredient != null)
                    _lookup.TryAdd(ingredient.Type, ingredient);
            }
        }
    }
}