using Mergeburgers.Events;
using Mergeburgers.Tools;
using Mergeburgers.Yandex;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Mergeburgers.Core
{
  public sealed class GameBootstrap : MonoBehaviour
  {
    private const string MAIN_MENU_SCENE = "MainMenu";

    [SerializeField] private string _nextScene = MAIN_MENU_SCENE;
    
    private IAdService _adService;
    private SignalBus _signalBus;
    private SaveManager _saveManager;

    [Inject]
    public void Construct(
      IAdService adService, 
      SignalBus signalBus,
      SaveManager saveManager)
    {
      _adService = adService;
      _signalBus = signalBus;
      _saveManager = saveManager;
    }
    
    private async void Start()
    {
      // Smoke test DI: убеждаемся, что инжект работает
      await _saveManager.LoadAsync();
      Debug.Log($"[Bootstrap] Save ready: coins={_saveManager.Current.coins}, energy={_saveManager.Current.energy}");

      Debug.Log($"[Bootstrap] Ad service ready: {_adService.IsReady}");
      
      // Smoke test SignalBus — DebugEventLogger должен поймать и залогировать
      _signalBus.Fire(new LevelMilestoneSignal(0));
      
      await SceneManager.LoadSceneAsync(_nextScene);
    }
  }
}