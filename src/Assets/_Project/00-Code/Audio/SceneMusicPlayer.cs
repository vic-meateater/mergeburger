using UnityEngine;
using Zenject;

namespace Mergeburgers.Audio
{
  public enum SceneMusicType
  {
    Menu,
    Game,
    None
  }

  /// <summary>
  /// Висит на сцене Game или MainMenu. В Start запускает нужную музыку.
  /// </summary>
  public sealed class SceneMusicPlayer : MonoBehaviour
  {
    [SerializeField] private SceneMusicType _type = SceneMusicType.Menu;

    private AudioService _audio;

    [Inject]
    public void Construct(AudioService audio)
    {
      _audio = audio;
    }

    private void Start()
    {
      switch (_type)
      {
        case SceneMusicType.Menu: _audio.PlayMenuMusic(); break;
        case SceneMusicType.Game: _audio.PlayGameMusic(); break;
      }
    }
  }
}