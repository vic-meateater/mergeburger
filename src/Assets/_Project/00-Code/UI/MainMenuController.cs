using Mergeburgers.Core;
using Mergeburgers.Meta;
using Mergeburgers.Tools;
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

    private EnergyManager _energy;
    private EconomyManager _economy;

    [Inject]
    public void Construct(EnergyManager energy, EconomyManager economy)
    {
      _energy = energy;
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
        Debug.Log($"[MainMenu] Cashed out previous session: +{cashedOut}");
      }
    }

    private async void OnPlayClicked()
    {
      if (!_energy.TrySpend(1))
      {
        Debug.Log("[MainMenu] No energy");
        return;
      }

      await SceneManager.LoadSceneAsync(GAME_SCENE);
    }

    private void OnDestroy()
    {
      _playButton.onClick.RemoveListener(OnPlayClicked);
    }
  }
}