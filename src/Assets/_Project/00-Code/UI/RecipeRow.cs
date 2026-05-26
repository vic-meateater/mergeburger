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
    [SerializeField] private float _iconSize = 80f;
    [SerializeField] private float _plusWidth = 20f;
    [SerializeField] private float _spacingBetween = 6f;

    [Header("Containers")] 
    [SerializeField] private Transform _ingredientsContainer;

    [SerializeField] private Image _resultIcon;
    [SerializeField] private Sprite _closedBurgerSprite;
    [SerializeField] private TextMeshProUGUI _priceLabel;
    [SerializeField] private GameObject _lockedOverlay;
    [SerializeField] private TextMeshProUGUI _requirementLabel;


    [Header("Prefabs")] 
    [SerializeField] private Image _ingredientIconPrefab;
    [SerializeField] private TextMeshProUGUI _plusSymbolPrefab;

    public void Setup(RecipeData recipe, IngredientDatabase ingredientDb, bool isUnlocked)
    {
      // Очищаем старые
      foreach (Transform child in _ingredientsContainer)
        Destroy(child.gameObject);

      // Спавним иконки и плюсы с ручными позициями
      float cursorX = 0f;

      for (int i = 0; i < recipe.Pattern.Count; i++)
      {
        var ingredientType = recipe.Pattern[i];
        var data = ingredientDb.Get(ingredientType);

        // Иконка
        var icon = Instantiate(_ingredientIconPrefab, _ingredientsContainer);
        var iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.pivot = new Vector2(0, 0.5f);
        iconRect.sizeDelta = new Vector2(_iconSize, _iconSize);
        iconRect.anchoredPosition = new Vector2(cursorX, 0);

        if (data?.Icon != null)
        {
          icon.sprite = data.Icon;
          icon.color = Color.white;
        }

        cursorX += _iconSize + _spacingBetween;

        // Плюс между ингредиентами (не после последнего)
        if (i < recipe.Pattern.Count - 1)
        {
          var plus = Instantiate(_plusSymbolPrefab, _ingredientsContainer);
          var plusRect = plus.GetComponent<RectTransform>();
          plusRect.anchorMin = new Vector2(0, 0.5f);
          plusRect.anchorMax = new Vector2(0, 0.5f);
          plusRect.pivot = new Vector2(0, 0.5f);
          plusRect.sizeDelta = new Vector2(_plusWidth, _iconSize);
          plusRect.anchoredPosition = new Vector2(cursorX, 0);
          plus.text = "+";

          cursorX += _plusWidth + _spacingBetween;
        }
      }

      if (isUnlocked)
      {
        // Результат
        var resultData = ingredientDb.Get(recipe.Result);
        if (resultData?.Icon != null)
        {
          _resultIcon.sprite = resultData.Icon;
          _resultIcon.color = Color.white;
          _resultIcon.enabled = true;
        }

        //Цена бургера
        //_priceLabel.text = $"{recipe.BaseSellPrice}";
        // King Burger разблокирован — но всё равно подсказка про ★★★
        if (recipe.MinLevelRequired > 0 && _requirementLabel != null)
        {
          _requirementLabel.text = "* * *";
          _requirementLabel.gameObject.SetActive(true);
        }
        else if (_requirementLabel != null)
        {
          _requirementLabel.gameObject.SetActive(false);
        }
      }
      else
      {
        // Заблокирован: знак вопроса вместо иконки
        _resultIcon.sprite = _closedBurgerSprite;
        //_priceLabel.text = "???";

        if (_requirementLabel != null)
        {
          if (recipe.MinLevelRequired > 0)
            _requirementLabel.text = "* * *";
          else
            _requirementLabel.text = "?";
          _requirementLabel.gameObject.SetActive(true);
        }
      }

      // Overlay скрываем — задача через ??? и подпись
      _lockedOverlay.SetActive(false);
    }
  }
}