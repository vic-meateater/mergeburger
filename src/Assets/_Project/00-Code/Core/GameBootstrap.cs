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

    [Inject]
    public void Construct(IAdService adService, ICloudSaveService cloudSaveService)
    {
      _saveService = cloudSaveService;
      _adService = adService;
    }
    
    private async void Start()
    {
      // Smoke test DI: убеждаемся, что инжект работает
      var save = await _saveService.LoadAsync();
      Debug.Log($"[Bootstrap] Save loaded: {(string.IsNullOrEmpty(save) ? "<no save>" : "exists")}");

      Debug.Log($"[Bootstrap] Ad service ready: {_adService.IsReady}");
      SceneManager.LoadSceneAsync(_nextScene);
    }
  }
}