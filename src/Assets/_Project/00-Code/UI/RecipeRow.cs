using Mergeburgers.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mergeburgers.UI
{
  /// <summary>
  /// Одна строка рецепта в попапе: [bun]+[patty]+[bun] = [hamburger]  10 coins
  /// Иконки динамически создаются по Pattern рецепта.
  /// </summary>
  public sealed class RecipeRow : MonoBehaviour
  {
    [Header("Containers")] 
    [SerializeField] private Transform _ingredientsContainer;

    [SerializeField] private Image _resultIcon;
    [SerializeField] private TextMeshProUGUI _priceLabel;
    [SerializeField] private GameObject _lockedOverlay;

    [Header("Prefabs")] 
    [SerializeField] private Image _ingredientIconPrefab;
    [SerializeField] private TextMeshProUGUI _plusSymbolPrefab;

    public void Setup(RecipeData recipe, IngredientDatabase ingredientDb)
    {
      // Очищаем старые иконки
      foreach (Transform child in _ingredientsContainer)
        Destroy(child.gameObject);

      // Спавним иконки ингредиентов через "+"
      for (int i = 0; i < recipe.Pattern.Count; i++)
      {
        var ingredientType = recipe.Pattern[i];
        var data = ingredientDb.Get(ingredientType);

        var icon = Instantiate(_ingredientIconPrefab, _ingredientsContainer);
        if (data?.Icon != null)
        {
          icon.sprite = data.Icon;
          icon.color = Color.white;
        }

        // Плюс между ингредиентами (кроме после последнего)
        if (i < recipe.Pattern.Count - 1)
        {
          var plus = Instantiate(_plusSymbolPrefab, _ingredientsContainer);
          plus.text = "+";
        }
      }

      // Результат
      var resultData = ingredientDb.Get(recipe.Result);
      if (resultData?.Icon != null)
      {
        _resultIcon.sprite = resultData.Icon;
        _resultIcon.color = Color.white;
      }

      _priceLabel.text = $"{recipe.BaseSellPrice}";

      // King Burger показывается заблокированным со звёздами
      bool isLocked = recipe.MinLevelRequired > 0;
      _lockedOverlay.SetActive(isLocked);
    }
  }
}