using Mergeburgers.Data;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Gameplay
{
    public sealed class Board : MonoBehaviour
    {
        [Header("Layout")] 
        [SerializeField] private RectTransform _container;
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private int _width = 5;
        [SerializeField] private int _height = 5;
        [SerializeField] private float _tileSize = 160f;
        [SerializeField] private float _spacing = 8f;

        private IngredientDatabase _ingredientDatabase;
        private MergeResolver _mergeResolver;

        private Tile[,] _grid;
        private IngredientType[,] _state;

        [Inject]
        public void Construct(IngredientDatabase ingredientDatabase, MergeResolver mergeResolver)
        {
            _ingredientDatabase = ingredientDatabase;
            _mergeResolver = mergeResolver;
        }

        private void Start()
        {
            SpawnGrid();
        }

        public void HandleSwipe(SwipeDirection direction)
        {
            var result = _mergeResolver.Resolve(_state, direction);

            if (!result.AnyChange)
            {
                Debug.Log($"[Board] Swipe {direction} — no change");
                return;
            }

            _state = result.NewState;
            RedrawGrid();
            Debug.Log($"[Board] Swipe {direction} — {result.Operations.Count} ops");
        }

        private void SpawnGrid()
        {
            _grid = new Tile[_width, _height];
            _state = new IngredientType[_width, _height];
            
            var ingredients = _ingredientDatabase.All;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var tile = Instantiate(_tilePrefab, _container);
                    tile.BoardPosition = new Vector2Int(x, y);

                    var rect = tile.GetComponent<RectTransform>();
                    rect.anchoredPosition = CalculateTilePosition(x, y);
                    rect.sizeDelta = new Vector2(_tileSize, _tileSize);

                    // Случайный ингредиент из доступных (исключая None)
                    var randomIngredient = ingredients[Random.Range(1, ingredients.Count)];
                    _state[x, y] = randomIngredient.Type;
                    tile.SetIngredient(randomIngredient);

                    _grid[x, y] = tile;
                }
            }

            Debug.Log($"[Board] Spawned {_width}x{_height} grid");
        }

        private void RedrawGrid()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var type = _state[x, y];
                    var data = type == IngredientType.None ? null : _ingredientDatabase.Get(type);
                    _grid[x, y].SetIngredient(data);
                }
            }
        }

        private Vector2 CalculateTilePosition(int x, int y)
        {
            // Центрируем доску относительно (0,0) контейнера
            float totalSize = (_tileSize + _spacing);
            float offsetX = -((_width - 1) * totalSize) / 2f;
            float offsetY = -((_height - 1) * totalSize) / 2f;
            return new Vector2(offsetX + x * totalSize, offsetY + y * totalSize);
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height) return null;
            return _grid[x, y];
        }
    }
}