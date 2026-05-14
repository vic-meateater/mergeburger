using Mergeburgers.Meta;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Core
{
  /// <summary>
  /// MonoBehaviour, висящий на ProjectContext.prefab или на стартовой сцене.
  /// Ловит OnApplicationFocus(false) и OnApplicationPause(true) — это сигналы что
  /// игрок свернул вкладку или закрывает игру. Форсим сохранение чтобы прогресс не сгорел.
  /// </summary>
  public sealed class AppLifecycleHandler : MonoBehaviour
  {
    private AutosaveDispatcher _autosave;

    [Inject]
    public void Construct(AutosaveDispatcher autosave)
    {
      _autosave = autosave;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
      if (!hasFocus)
        _autosave?.ForceSave("focus_lost");
    }

    private void OnApplicationPause(bool pauseStatus)
    {
      if (pauseStatus)
        _autosave?.ForceSave("app_paused");
    }

    private void OnApplicationQuit()
    {
      _autosave?.ForceSave("app_quit");
    }
  }
}