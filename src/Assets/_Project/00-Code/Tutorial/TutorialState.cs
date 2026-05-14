namespace Mergeburgers.Tutorial
{
  public enum TutorialState
  {
    Inactive = 0,           // туториал не нужен (пройден ранее или пропущен)
    SwipeAny = 1,           // ждём первого свайпа
    MergeAny = 2,           // ждём первого merge (любого)
    BurgerFirst = 3,        // ждём первого BurgerCreated
    Complete = 4            // показан финальный попап
  }
}