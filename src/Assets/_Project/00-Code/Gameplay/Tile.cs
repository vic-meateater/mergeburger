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
        
        public IngredientType Type { get; private set; }
        public Vector2Int BoardPosition { get; set; }

        public void SetIngredient(IngredientData data)
        {
            if (data == null)
            {
                Type = IngredientType.None;
                _background.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                _letter.text = string.Empty;
                return;
            }
            
            Type = data.Type;
            _background.color = data.PlaceholderColor;
            _letter.text = data.PlaceholderLetter;
        }
    }
}