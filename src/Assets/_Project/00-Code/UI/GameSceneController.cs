using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mergeburgers.UI
{
  public sealed class GameSceneController : MonoBehaviour
  {
    private const string MAIN_MENU_SCENE = "MainMenu";

    [SerializeField] private Button _backButton;

    private void Awake()
    {
      _backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnBackClicked()
    {
      SceneManager.LoadSceneAsync(MAIN_MENU_SCENE);
    }

    private void OnDestroy()
    {
      _backButton.onClick.RemoveListener(OnBackClicked);
    }
  }
}