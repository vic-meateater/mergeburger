using Mergeburgers.Core;
using Mergeburgers.Data;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Mergeburgers.UI
{
  public sealed class BurgerShelf : MonoBehaviour
  {
    [SerializeField] private Image _slotPrefab; // префаб одной "ячейки полки"
    [SerializeField] private Transform _slotsContainer;
    [SerializeField] private Sprite _emptySlotSprite; // серый круг "не разблокировано"

    private SaveManager _saveManager;
    private IngredientDatabase _ingredientDatabase;
    private RecipeDatabase _recipeDatabase;

    [Inject]
    public void Construct(SaveManager saveManager, IngredientDatabase ingredientDatabase, RecipeDatabase recipeDatabase)
    {
      _saveManager = saveManager;
      _ingredientDatabase = ingredientDatabase;
      _recipeDatabase = recipeDatabase;
    }

    private void Start()
    {
      Populate();
    }

    private void Populate()
    {
      foreach (Transform child in _slotsContainer)
        Destroy(child.gameObject);

      var unlocked = _saveManager.Current?.unlockedRecipes ?? new System.Collections.Generic.List<string>();

      foreach (var recipe in _recipeDatabase.All)
      {
        var slot = Instantiate(_slotPrefab, _slotsContainer);
        bool isUnlocked = unlocked.Contains(recipe.DisplayName); // ← унифицировано по DisplayName

        if (isUnlocked)
        {
          var data = _ingredientDatabase.Get(recipe.Result);
          slot.sprite = data?.Icon;
          slot.color = Color.white;
        }
        else
        {
          // Пустой слот: показываем серый плейсхолдер или скрываем
          slot.sprite = _emptySlotSprite;
          slot.color = new Color(1f, 1f, 1f, 0.15f); // почти прозрачный — выемка пустая
        }
      }
    }
  }
}