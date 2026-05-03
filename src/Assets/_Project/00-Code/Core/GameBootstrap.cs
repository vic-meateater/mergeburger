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
    private ICloudSaveService _saveService;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(
      IAdService adService, 
      ICloudSaveService cloudSaveService,
      SignalBus signalBus)
    {
      _saveService = cloudSaveService;
      _adService = adService;
      _signalBus = signalBus;
    }
    
    private async void Start()
    {
      // Smoke test DI: убеждаемся, что инжект работает
      var save = await _saveService.LoadAsync();
      Debug.Log($"[Bootstrap] Save loaded: {(string.IsNullOrEmpty(save) ? "<no save>" : "exists")}");

      Debug.Log($"[Bootstrap] Ad service ready: {_adService.IsReady}");
      
      // Smoke test SignalBus — DebugEventLogger должен поймать и залогировать
      _signalBus.Fire(new LevelMilestoneSignal(0));
      
      await SceneManager.LoadSceneAsync(_nextScene);
    }
  }
}