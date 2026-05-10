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
    public int LifeRemaining { get; }
    public int BurgerSellPrice { get; }

    public IngredientCell(IngredientType type, int level, int lifeRemaining = 0, int burgerSellPrice = 0)
    {
      Type = type;
      Level = level;
      LifeRemaining = lifeRemaining;
      BurgerSellPrice = burgerSellPrice;
    }

    public bool IsEmpty => Type == IngredientType.None;

    public static IngredientCell Empty => new IngredientCell(IngredientType.None, 0, 0);

    public static IngredientCell Of(IngredientType type, int level = 1)
      => new IngredientCell(type, level, 0);
    
    /// <summary>
    /// Создаёт бургер с заданным сроком жизни. По умолчанию 3 хода.
    /// </summary>
    public static IngredientCell Burger(IngredientType type, int sellPrice, int lifeRemaining = 3)
      => new IngredientCell(type, 0, lifeRemaining, sellPrice);

    /// <summary>
    /// Возвращает копию с уменьшенным сроком жизни (для тика после хода).
    /// </summary>
    public IngredientCell WithLifeDecremented()
      => new IngredientCell(Type, Level, LifeRemaining - 1, BurgerSellPrice);

    public bool SameTypeAs(IngredientCell other) => Type == other.Type && !IsEmpty;
    public bool SameTypeAndLevel(IngredientCell other) => Type == other.Type && Level == other.Level && !IsEmpty;
  }
}