using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project._00_Code.UI
{
  public sealed class MainMenuController : MonoBehaviour
  {
    private const string GAME_SCENE = "Game";

    [SerializeField] private Button _playButton;

    private void Awake()
    {
      _playButton.onClick.AddListener(OnPlayClicked);
    }

    private void OnPlayClicked()
    {
      SceneManager.LoadSceneAsync(GAME_SCENE);
    }
    
    private void OnDestroy()
    {
      _playButton.onClick.RemoveListener(OnPlayClicked);
    }
  }
}