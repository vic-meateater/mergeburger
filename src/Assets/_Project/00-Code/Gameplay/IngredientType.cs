namespace Mergeburgers.Gameplay
{
    public enum IngredientType
    {
        None = 0,
        Bun = 1,
        Patty = 2,
        Cheese = 3,
        Lettuce = 4,
        Tomato = 5,
        Sauce = 6,
        
        Hamburger = 100,
        Cheeseburger = 101,
        Veggieburger = 102,
        BigMac = 103,
        KingBurger = 200
    }
    
    public static class IngredientTypeExtensions
    {
        /// <summary>
        /// Базовые ингредиенты (1-99). Сливаются между собой по правилу 2/3+.
        /// </summary>
        public static bool IsBaseIngredient(this IngredientType type)
            => (int)type > 0 && (int)type < 100;

        /// <summary>
        /// Готовые бургеры (100+). Не сливаются, движутся до препятствия.
        /// </summary>
        public static bool IsBurger(this IngredientType type)
            => (int)type >= 100;
    }
}