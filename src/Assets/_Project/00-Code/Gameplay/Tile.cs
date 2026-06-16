using System.Collections.Generic;
using Mergeburgers.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mergeburgers.Gameplay
{
  public sealed class Tile : MonoBehaviour
  {
    [SerializeField] private Image _background;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _letter;
    [SerializeField] private TextMeshProUGUI _levelLabel;

    public IngredientType Type { get; private set; }
    public int Level { get; private set; }
    public Vector2Int BoardPosition { get; set; }

    private List<RectTransform> _contentTransforms;

    /// <summary>
    /// Визуальное содержимое плитки (иконка/буква/уровень) без фона-ячейки.
    /// Анимация движения двигает только это, чтобы фон-ячейка оставалась на месте.
    /// </summary>
    public IReadOnlyList<RectTransform> ContentTransforms
    {
      get
      {
        if (_contentTransforms == null)
        {
          _contentTransforms = new List<RectTransform>(3);
          if (_iconImage != null) _contentTransforms.Add((RectTransform)_iconImage.transform);
          if (_letter != null) _contentTransforms.Add((RectTransform)_letter.transform);
          if (_levelLabel != null) _contentTransforms.Add((RectTransform)_levelLabel.transform);
        }

        return _contentTransforms;
      }
    }

    public void SetCell(IngredientCell cell, IngredientData data)
    {
      if (cell.IsEmpty || data == null)
      {
        Type = IngredientType.None;
        Level = 0;
        _iconImage.sprite = null;
        _iconImage.enabled = false;
        _letter.text = string.Empty;
        if (_levelLabel != null) _levelLabel.text = string.Empty;
        return;
      }

      Type = cell.Type;
      Level = cell.Level;

      if (data.Icon != null)
      {
        _iconImage.sprite = data.Icon;
        _iconImage.enabled = true;
        _letter.text = string.Empty;
      }
      else
      {
        // Fallback: если иконки нет, показываем букву на фоне placeholderColor
        _iconImage.enabled = false;
        _letter.text = data.PlaceholderLetter;
        // Можно опционально менять _background.color для fallback, но лучше оставить серую сетку
      }

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