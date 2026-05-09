namespace Mergeburgers.Gameplay
{
  /// <summary>
  /// Одна клетка доски: тип ингредиента + уровень прокачки.
  /// None означает пустую клетку, Level для None игнорируется.
  /// Уровни: 1 (базовый), 2, 3 (cap).
  /// </summary>
  public readonly struct IngredientCell
  {
    public IngredientType Type { get; }
    public int Level { get; }

    public IngredientCell(IngredientType type, int level)
    {
      Type = type;
      Level = level;
    }

    public bool IsEmpty => Type == IngredientType.None;

    public static IngredientCell Empty => new IngredientCell(IngredientType.None, 0);

    public static IngredientCell Of(IngredientType type, int level = 1)
      => new IngredientCell(type, level);

    public bool SameTypeAs(IngredientCell other) => Type == other.Type && !IsEmpty;

    public bool SameTypeAndLevel(IngredientCell other) => Type == other.Type && Level == other.Level && !IsEmpty;
  }
}