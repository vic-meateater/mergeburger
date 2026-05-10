using Mergeburgers.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mergeburgers.Gameplay
{
  public sealed class Tile : MonoBehaviour
  {
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _letter;
    [SerializeField] private TextMeshProUGUI _levelLabel;

    public IngredientType Type { get; private set; }
    public int Level { get; private set; }
    public Vector2Int BoardPosition { get; set; }

    public void SetCell(IngredientCell cell, IngredientData data)
    {
      if (cell.IsEmpty || data == null)
      {
        Type = IngredientType.None;
        Level = 0;
        _background.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        _letter.text = string.Empty;
        if (_levelLabel != null) _levelLabel.text = string.Empty;
        return;
      }

      Type = cell.Type;
      Level = cell.Level;

      var color = data.PlaceholderColor;
      color.a = 1f;
      _background.color = color;
      _letter.text = data.PlaceholderLetter;

      if (_levelLabel != null)
      {
        // Бургеры показывают остаток жизни в углу. Базовые ингредиенты показывают звёзды.
        if (cell.Type.IsBurger())
        {
          _levelLabel.text = cell.LifeRemaining.ToString();
        }
        else
        {
          if (cell.Level == 1) _levelLabel.text = "*";
          else if (cell.Level == 2) _levelLabel.text = "**";
          else if (cell.Level == 3) _levelLabel.text = "***";
          else _levelLabel.text = string.Empty;
        }
      }
    }
  }
}