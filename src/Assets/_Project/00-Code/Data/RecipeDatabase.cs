using System.Collections.Generic;
using UnityEngine;

namespace Mergeburgers.Data
{
  [CreateAssetMenu(fileName = "RecipeDatabase", menuName = "Mergeburgers/Recipe Database", order = 0)]
  public sealed class RecipeDatabase : ScriptableObject
  {
    [field: SerializeField] public List<RecipeData> All { get; private set; }
  }
}