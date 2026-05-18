using Mergeburgers.Data;
using UnityEngine;
using Zenject;

namespace Mergeburgers.UI
{
  /// <summary>
  /// Постоянно видимая панель со всеми рецептами на сцене Game.
  /// King Burger показывается заблокированным если его ещё не собирали.
  /// </summary>
  public sealed class RecipeBookHud : MonoBehaviour
  {
    [SerializeField] private RecipeBookItem _itemPrefab;
    [SerializeField] private Transform _itemsContainer;

    private RecipeDatabase _recipeDatabase;
    private IngredientDatabase _ingredientDatabase;

    [Inject]
    public void Construct(RecipeDatabase recipeDatabase, IngredientDatabase ingredientDatabase)
    {
      _recipeDatabase = recipeDatabase;
      _ingredientDatabase = ingredientDatabase;
    }

    private void Start()
    {
      PopulateItems();
    }

    private void PopulateItems()
    {
      foreach (Transform child in _itemsContainer)
        Destroy(child.gameObject);

      foreach (var recipe in _recipeDatabase.All)
      {
        var item = Instantiate(_itemPrefab, _itemsContainer);
        var resultData = _ingredientDatabase.Get(recipe.Result);
        bool isLocked = recipe.MinLevelRequired > 0; // King Burger
        item.Setup(recipe, resultData, isLocked);
      }
    }
  }
}