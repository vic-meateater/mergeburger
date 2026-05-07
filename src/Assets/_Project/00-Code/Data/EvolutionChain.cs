using System.Collections.Generic;
using Mergeburgers.Gameplay;
using UnityEngine;

namespace Mergeburgers.Data
{
    /// <summary>
    /// Простая цепочка эволюции на День 10: при паре одинаковых ингредиентов получается следующий.
    /// Bun+Bun → Patty → Cheese → Lettuce → Tomato → Sauce → (max, не сливается).
    ///
    /// На Дне 12 эта линейная цепочка будет заменена на полноценное дерево рецептов
    /// (булка + котлета + булка = гамбургер, и так далее).
    /// </summary>
    [CreateAssetMenu(fileName = "EvolutionChain", menuName = "Mergeburgers/Evolution Chain", order = 2)]
    public sealed class EvolutionChain : ScriptableObject
    {
        [SerializeField] private List<IngredientType> _chain = new()
        {
            IngredientType.Bun,
            IngredientType.Patty,
            IngredientType.Cheese,
            IngredientType.Lettuce,
            IngredientType.Tomato,
            IngredientType.Sauce
        };

        /// <summary>
        /// Возвращает следующий тип в цепочке, или None если уже максимум.
        /// </summary>
        public IngredientType Evolve(IngredientType type)
        {
            int index = _chain.IndexOf(type);
            if (index < 0 || index >= _chain.Count - 1)
                return IngredientType.None;
            return _chain[index + 1];
        }
    }
}