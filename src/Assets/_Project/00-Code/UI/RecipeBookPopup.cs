using Mergeburgers.Data;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Mergeburgers.UI
{
  public sealed class RecipeBookPopup : MonoBehaviour
  {
    [SerializeField] private GameObject _root;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _backdrop; // тапнуть в тёмную зону - закрыть
    [SerializeField] private RecipeRow _rowPrefab;
    [SerializeField] private Transform _rowsContainer;

    private RecipeDatabase _recipeDatabase;
    private IngredientDatabase _ingredientDatabase;
    private bool _isPopulated;

    [Inject]
    public void Construct(RecipeDatabase recipeDatabase, IngredientDatabase ingredientDatabase)
    {
      _recipeDatabase = recipeDatabase;
      _ingredientDatabase = ingredientDatabase;
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
      if (!_isPopulated) Populate();
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

      foreach (var recipe in _recipeDatabase.All)
      {
        var row = Instantiate(_rowPrefab, _rowsContainer);
        row.Setup(recipe, _ingredientDatabase);
      }

      _isPopulated = true;
    }
  }
}