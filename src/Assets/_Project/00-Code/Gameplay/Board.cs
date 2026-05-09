using Mergeburgers.Events;
using Mergeburgers.Data;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Gameplay
{
  public sealed class Board : MonoBehaviour
  {
    [Header("Layout")] [SerializeField] private RectTransform _container;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 5;
    [SerializeField] private float _tileSize = 160f;
    [SerializeField] private float _spacing = 8f;

    private IngredientDatabase _ingredientDatabase;
    private MergeResolver _mergeResolver;
    private RecipeMatcher _recipeMatcher;
    private TileSpawner _tileSpawner;
    private GameOverChecker _gameOverChecker;
    private SignalBus _signalBus;

    private Tile[,] _grid;
    private IngredientCell[,] _state;

    private bool _isGameOver;

    [Inject]
    public void Construct(
      IngredientDatabase ingredientDatabase,
      MergeResolver mergeResolver,
      RecipeMatcher recipeMatcher,
      TileSpawner tileSpawner,
      GameOverChecker gameOverChecker,
      SignalBus signalBus
    )
    {
      _ingredientDatabase = ingredientDatabase;
      _mergeResolver = mergeResolver;
      _recipeMatcher = recipeMatcher;
      _tileSpawner = tileSpawner;
      _gameOverChecker = gameOverChecker;
      _signalBus = signalBus;
    }

    private void Start()
    {
      SpawnGrid();
    }

    public void HandleSwipe(SwipeDirection direction)
    {
      if (_isGameOver) return;

      // 1. Движение + эволюции пар
      var resolveResult = _mergeResolver.Resolve(_state, direction);
      if (!resolveResult.AnyChange)
      {
        Debug.Log($"[Board] Swipe {direction} — no change");
        return;
      }

      _state = resolveResult.NewState;

      // 2. Рецептурный merge
      // var matchResult = _recipeMatcher.FindAndApply(_state);
      // _state = matchResult.NewState;

      // foreach (var match in matchResult.Matches)
      // {
      //   Debug.Log(
      //     $"[Board] Recipe matched: {match.Recipe.DisplayName} at {match.CenterPosition} (+{match.Recipe.SellPrice})");
      //   _signalBus.Fire(new BurgerCreatedSignal(
      //     recipeId: match.Recipe.DisplayName,
      //     reward: match.Recipe.SellPrice,
      //     boardPosition: match.CenterPosition
      //   ));
      // }

      // 3. Спавн новой плитки
      _tileSpawner.SpawnOne(_state);

      // 4. Перерисовка
      RedrawGrid();

      // 5. Game Over check
      if (_gameOverChecker.IsGameOver(_state))
      {
        _isGameOver = true;
        Debug.Log("[Board] GAME OVER");
        _signalBus.Fire(new GameOverSignal(finalScore: 0, burgersCreated: 0));
      }
    }

    private void SpawnGrid()
    {
      _grid = new Tile[_width, _height];
      _state = new IngredientCell[_width, _height];
      
      for (int x = 0; x < _width; x++)
      for (int y = 0; y < _height; y++)
      {
        var tile = Instantiate(_tilePrefab, _container);
        tile.BoardPosition = new Vector2Int(x, y);

        var rect = tile.GetComponent<RectTransform>();
        rect.anchoredPosition = CalculateTilePosition(x, y);
        rect.sizeDelta = new Vector2(_tileSize, _tileSize);

        if (Random.value < 0.6f)
          _tileSpawner.SpawnOne(_state);

        _grid[x, y] = tile;
      }

      RedrawGrid();
      Debug.Log($"[Board] Spawned {_width}x{_height} grid");
    }

    private void RedrawGrid()
    {
      for (int x = 0; x < _width; x++)
      for (int y = 0; y < _height; y++)
      {
        var cell  = _state[x, y];
        var data = cell.IsEmpty ? null : _ingredientDatabase.Get(cell.Type);
        _grid[x, y].SetCell(cell, data);
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
  }
}