using Mergeburgers.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mergeburgers.UI
{
  public sealed class RecipeBookItem : MonoBehaviour
  {
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _priceLabel;
    [SerializeField] private GameObject _lockedOverlay;

    public void Setup(RecipeData recipe, IngredientData resultIngredient, bool isLocked)
    {
      if (resultIngredient?.Icon != null)
        _icon.sprite = resultIngredient.Icon;

      _nameLabel.text = recipe.DisplayName;
      _priceLabel.text = $"{recipe.BaseSellPrice}";
      _lockedOverlay.SetActive(isLocked);
    }
  }
}