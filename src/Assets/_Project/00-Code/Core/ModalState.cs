namespace Mergeburgers.Core
{
  /// <summary>
  /// Централизованный счётчик открытых модальных окон.
  /// Любой попап (GameOverPopup, TutorialHud completion и т.д.) делает Push при показе
  /// и Pop при скрытии. Игровой ввод (свайпы доски) игнорируется, пока IsAnyOpen.
  /// </summary>
  public sealed class ModalState
  {
    private int _count;

    public bool IsAnyOpen => _count > 0;

    public void Push() => _count++;

    public void Pop()
    {
      if (_count > 0) _count--;
    }
  }
}
