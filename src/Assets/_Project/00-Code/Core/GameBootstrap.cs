using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project._00_Code.Core
{
  public sealed class GameBootstrap : MonoBehaviour
  {
    private const string MAIN_MENU_SCENE = "MainMenu";

    [SerializeField] private string _nextScene = MAIN_MENU_SCENE;

    private void Start()
    {
      SceneManager.LoadSceneAsync(_nextScene);
    }
  }
}