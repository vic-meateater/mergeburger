using Mergeburgers.Yandex;

using Zenject;

#if UNITY_EDITOR
using Mergeburgers.Yandex.Editor;
#endif

namespace Mergeburgers.Core
{
  public class GameInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
#if UNITY_EDITOR
      YandexServices();
#else
            // TODO День 6: WebGL-имплементации через max-games плагин
#endif
    }

#if UNITY_EDITOR
    private void YandexServices()
    {
      Container.Bind<IAdService>().To<EditorAdService>().AsSingle();
      Container.Bind<ICloudSaveService>().To<EditorCloudSaveService>().AsSingle();
      Container.Bind<ILeaderboardService>().To<EditorLeaderboardService>().AsSingle();
      Container.Bind<IIAPService>().To<EditorIAPService>().AsSingle();
    }
#endif
  }
}