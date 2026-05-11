using System;
using Mergeburgers.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Mergeburgers.UI
{
  public sealed class MainMenuController : MonoBehaviour
  {
    private const string GAME_SCENE = "Game";

    [SerializeField] private Button _playButton;

    private EconomyManager _economy;

    [Inject]
    public void Construct(EconomyManager economy)
    {
      _economy = economy;
    }

    private void Awake()
    {
      _playButton.onClick.AddListener(OnPlayClicked);
    }

    private void Start()
    {
      int cashedOut = _economy.CashOut();
      if (cashedOut > 0)
      {
        // TODO Day 17.B: показать popup "Раунд завершён, заработано +N"
      }
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