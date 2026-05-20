using System.Collections.Generic;
using System.Threading.Tasks;
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

    public int CurrentSessionCoins { get; private set; }
    public bool IsGameOver => _isGameOver;

    private IngredientDatabase _ingredientDatabase;
    private MergeResolver _mergeResolver;
    private RecipeMatcher _recipeMatcher;
    private TileSpawner _tileSpawner;
    private GameOverChecker _gameOverChecker;
    private BurgerLifecycle _burgerLifecycle;
    private BoardAnimator _animator;
    private SignalBus _signalBus;

    private Tile[,] _grid;
    private IngredientCell[,] _state;
    private Vector2[,] _targetPositions; // кеш позиций каждой клетки доски
    private bool _isGameOver;
    private bool _isAnimating;

    [Inject]
    public void Construct(
      IngredientDatabase ingredientDatabase,
      MergeResolver mergeResolver,
      RecipeMatcher recipeMatcher,
      TileSpawner tileSpawner,
      GameOverChecker gameOverChecker,
      BurgerLifecycle burgerLifecycle,
      BoardAnimator animator,
      SignalBus signalBus
    )
    {
      _ingredientDatabase = ingredientDatabase;
      _mergeResolver = mergeResolver;
      _recipeMatcher = recipeMatcher;
      _tileSpawner = tileSpawner;
      _gameOverChecker = gameOverChecker;
      _burgerLifecycle = burgerLifecycle;
      _animator = animator;
      _signalBus = signalBus;
    }

    private void Start()
    {
      SpawnGrid();
    }

    public async void HandleSwipe(SwipeDirection direction)
    {
      if (_isGameOver || _isAnimating) return;

      var resolveResult = _mergeResolver.Resolve(_state, direction);
      if (!resolveResult.AnyChange)
      {
        Debug.Log($"[Board] Swipe {direction} — no change");
        return;
      }

      _isAnimating = true;

      try
      {
        // 1. Анимация движения по операциям резолвера
        await _animator.PlayMoveOperations(resolveResult.Operations, _grid, _targetPositions);

        // 2. Применяем new state от resolver
        _state = resolveResult.NewState;

        // MergeOccurred для туториала (если был хоть один merge)
        foreach (var op in resolveResult.Operations)
          if (op.Type == MergeOperationType.Merge)
          {
            _signalBus.Fire(new MergeOccurredSignal());
            break;
          }

        ResetTilePositions();
        RedrawGrid();
        
        // 3. НАЙТИ бургеры на продажу (но НЕ удалять пока)
        var expiring = _burgerLifecycle.FindExpiringBurgers(_state);

        // 4. Анимировать продажу на этих плитках ДО reset positions
        var sellTasks = new List<Task>();
        foreach (var ex in expiring)
        {
          var tile = _grid[ex.Position.x, ex.Position.y];
          sellTasks.Add(_animator.PlayBurgerSold(tile));
        }
        if (sellTasks.Count > 0)
          await Task.WhenAll(sellTasks);

        // 5. ПРИМЕНИТЬ тик (теперь state очищен от проданных)
        _state = _burgerLifecycle.ApplyTick(_state);

        // 6. Рецепты
        var matchResult = _recipeMatcher.FindAndApply(_state);
        _state = matchResult.NewState;

        //ResetTilePositions();
        RedrawGrid();

        foreach (var match in matchResult.Matches)
        {
          Debug.Log($"[Board] Recipe matched: {match.Recipe.DisplayName} ×{match.Multiplier}");
          _signalBus.Fire(new BurgerCreatedSignal(
            recipeId: match.Recipe.DisplayName,
            reward: match.FinalPrice,
            boardPosition: match.CenterPosition
          ));

          var burgerTile = _grid[match.CenterPosition.x, match.CenterPosition.y];
          await _animator.PlayBurgerCreated(burgerTile);
        }

        _tileSpawner.SpawnOne(_state);
        RedrawGrid();

        if (_gameOverChecker.IsGameOver(_state))
        {
          _isGameOver = true;
          Debug.Log("[Board] GAME OVER");
          _signalBus.Fire(new GameOverSignal(0, 0));
        }
      }
      finally
      {
        _isAnimating = false;
      }
    }

    public void ClearRandomTilesAndContinue(int count)
    {
      var nonEmptyPositions = new List<Vector2Int>();
      for (int x = 0; x < _width; x++)
      for (int y = 0; y < _height; y++)
        if (!_state[x, y].IsEmpty)
          nonEmptyPositions.Add(new Vector2Int(x, y));

      // Берём count случайных, или все если меньше
      int toRemove = Mathf.Min(count, nonEmptyPositions.Count);
      for (int i = 0; i < toRemove; i++)
      {
        int idx = Random.Range(0, nonEmptyPositions.Count);
        var pos = nonEmptyPositions[idx];
        _state[pos.x, pos.y] = IngredientCell.Empty;
        nonEmptyPositions.RemoveAt(idx);
      }

      _isGameOver = false;
      RedrawGrid();
      Debug.Log($"[Board] Cleared {toRemove} tiles, continuing");
    }
    
    public Vector2 GetTileAnchoredPosition(Vector2Int boardPos)
    {
      if (boardPos.x < 0 || boardPos.x >= _width || boardPos.y < 0 || boardPos.y >= _height)
        return Vector2.zero;
      return _targetPositions[boardPos.x, boardPos.y];
    }

    /// <summary>
    /// После перемещения через анимации Tile-объекты в _grid находятся не на своих "сетка-позициях".
    /// Этот метод приводит координаты grid[x,y] к ожидаемому target[x,y] и одновременно перепривязывает
    /// Tile-объекты в _grid согласно новому состоянию (т.е. правильный Tile в правильной клетке).
    ///
    /// Простая версия: всегда возвращаем все плитки на их сетка-позиции по координатам в массиве _grid.
    /// </summary>
    private void ResetTilePositions()
    {
      for (int x = 0; x < _width; x++)
      for (int y = 0; y < _height; y++)
      {
        var tile = _grid[x, y];
        if (tile == null) continue;
        var rect = tile.GetComponent<RectTransform>();
        rect.anchoredPosition = _targetPositions[x, y];
      }
    }

    private void SpawnGrid()
    {
      _grid = new Tile[_width, _height];
      _state = new IngredientCell[_width, _height];
      _targetPositions = new Vector2[_width, _height];

      for (int x = 0; x < _width; x++)
      for (int y = 0; y < _height; y++)
      {
        var tile = Instantiate(_tilePrefab, _container);
        tile.BoardPosition = new Vector2Int(x, y);

        var rect = tile.GetComponent<RectTransform>();
        var pos = CalculateTilePosition(x, y);
        _targetPositions[x, y] = pos;
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(_tileSize, _tileSize);

        if (Random.value < 0.5f)
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
        var cell = _state[x, y];
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