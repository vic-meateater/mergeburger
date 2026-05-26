using Mergeburgers.Core;
using Mergeburgers.Events;
using Mergeburgers.Gameplay;
using Mergeburgers.Tools;
using Mergeburgers.Yandex;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Mergeburgers.UI
{
  /// <summary>
  /// Game Over popup с двумя действиями: cashout (вернуться в меню) и rewarded continue.
  /// Открывается по GameOverSignal, при rewarded — даёт Board.ClearRandomTilesAndContinue.
  /// </summary>
  public sealed class GameOverPopup : MonoBehaviour
  {
    private const string RewardedPlacementId = "continue_5_tiles";
    private const int TilesToClear = 5;
    private const string MainMenuScene = "MainMenu";

    [Header("UI")] 
    [SerializeField] private GameObject _root; // вся панель попапа
    [SerializeField] private TextMeshProUGUI _earnedLabel;
    [SerializeField] private Button _claimButton;
    [SerializeField] private Button _continueButton;

    private SignalBus _signalBus;
    private EconomyManager _economy;
    private IAdService _adService;
    private Board _board;

    private bool _isOpen;

    [Inject]
    public void Construct(
      SignalBus signalBus,
      EconomyManager economy,
      IAdService adService,
      Board board)
    {
      _signalBus = signalBus;
      _economy = economy;
      _adService = adService;
      _board = board;
    }

    private void Start()
    {
      _root.SetActive(false);
      _claimButton.onClick.AddListener(OnClaimClicked);
      _continueButton.onClick.AddListener(OnContinueClicked);

      _signalBus.Subscribe<GameOverSignal>(OnGameOver);
    }

    private void OnDestroy()
    {
      if (_signalBus != null)
        _signalBus.Unsubscribe<GameOverSignal>(OnGameOver);

      _claimButton.onClick.RemoveListener(OnClaimClicked);
      _continueButton.onClick.RemoveListener(OnContinueClicked);
    }

    private void OnGameOver(GameOverSignal s)
    {
      if (_isOpen) return;

      _isOpen = true;
      _root.SetActive(true);
      _earnedLabel.text = $"Заработано: {_economy.CurrentSession}";
    }

    private async void OnClaimClicked()
    {
      // Cashout и возврат в меню
      var amount = _economy.CashOut();
      _root.SetActive(false);
      _isOpen = false;
      await SceneManager.LoadSceneAsync(MainMenuScene);
    }

    private async void OnContinueClicked()
    {
      _continueButton.interactable = false;

      var result = await _adService.ShowRewardedAsync(RewardedPlacementId);
      _continueButton.interactable = true;

      if (result == AdResult.Success)
      {
        _board.ClearRandomTilesAndContinue(TilesToClear);
        _root.SetActive(false);
        _isOpen = false;
      }
      else
      {
        // popup остаётся открытым, игрок может попробовать ещё раз или забрать награду
      }
    }
  }
}