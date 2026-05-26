using Mergeburgers.Core;
using Mergeburgers.Data;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Mergeburgers.UI
{
  public sealed class RecipeBookPopup : MonoBehaviour
  {
    private const float RowHeight = 120f;
    private const float RowSpacing = 14f;

    [SerializeField] private GameObject _root;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _backdrop; // тапнуть в тёмную зону - закрыть
    [SerializeField] private RecipeRow _rowPrefab;
    [SerializeField] private Transform _rowsContainer;

    private RecipeDatabase _recipeDatabase;
    private IngredientDatabase _ingredientDatabase;
    private SaveManager _saveManager;
    private bool _isPopulated;

    [Inject]
    public void Construct(
      RecipeDatabase recipeDatabase,
      IngredientDatabase ingredientDatabase,
      SaveManager saveManager)
    {
      _recipeDatabase = recipeDatabase;
      _ingredientDatabase = ingredientDatabase;
      _saveManager = saveManager;
    }

    private void Start()
    {
      _root.SetActive(false);
      _openButton.onClick.AddListener(Open);
      _closeButton.onClick.AddListener(Close);
      if (_backdrop != null) _backdrop.onClick.AddListener(Close);
    }

    private void OnDestroy()
    {
      _openButton.onClick.RemoveListener(Open);
      _closeButton.onClick.RemoveListener(Close);
      if (_backdrop != null) _backdrop.onClick.RemoveListener(Close);
    }

    private void Open()
    {
      Populate();
      _root.SetActive(true);
    }

    private void Close()
    {
      _root.SetActive(false);
    }

    private void Populate()
    {
      foreach (Transform child in _rowsContainer)
        Destroy(child.gameObject);

      var unlocked = _saveManager.Current?.unlockedRecipes ?? new System.Collections.Generic.List<string>();

      float cursorY = 0f;
      foreach (var recipe in _recipeDatabase.All)
      {
        var row = Instantiate(_rowPrefab, _rowsContainer);
        var rect = row.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1); // top-stretch
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.sizeDelta = new Vector2(0, RowHeight);
        rect.anchoredPosition = new Vector2(0, -cursorY);

        bool isUnlocked = unlocked.Contains(recipe.DisplayName);
        row.Setup(recipe, _ingredientDatabase, isUnlocked);
        cursorY += RowHeight + RowSpacing;
      }

      _isPopulated = true;
    }
  }
}