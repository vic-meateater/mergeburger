using Mergeburgers.Events;
using Mergeburgers.Yandex;

#if !UNITY_EDITOR
using Mergeburgers.Yandex.WebGL;
using Mergeburgers.Yandex.Editor;
#endif

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
      YandexServicesEditor();
#else
      YandexServicesWebGL();
#endif
      Signals();
      Managers();
      Dev();
    }

    

#if UNITY_EDITOR
    private void YandexServicesEditor()
    {
      Container.Bind<IAdService>().To<EditorAdService>().AsSingle();
      Container.Bind<ICloudSaveService>().To<EditorCloudSaveService>().AsSingle();
      Container.Bind<ILeaderboardService>().To<EditorLeaderboardService>().AsSingle();
      Container.Bind<IIAPService>().To<EditorIAPService>().AsSingle();
    }
#else
    private void YandexServicesWebGL()
    {
      Container.Bind<IAdService>().To<WebGLAdService>().AsSingle();
      Container.Bind<ICloudSaveService>().To<EditorCloudSaveService>().AsSingle();
      Container.Bind<ILeaderboardService>().To<EditorLeaderboardService>().AsSingle();
      Container.Bind<IIAPService>().To<EditorIAPService>().AsSingle();
    }
#endif
    
    private void Signals()
    {
      // 1. Установка SignalBus в контейнер
      SignalBusInstaller.Install(Container);

      // 2. Декларация сигналов — без этого Fire упадёт
      Container.DeclareSignal<GameOverSignal>();
      Container.DeclareSignal<BurgerCreatedSignal>();
      Container.DeclareSignal<EnergySpentSignal>();
      Container.DeclareSignal<LevelMilestoneSignal>();
    }
    
    private void Managers()
    {
      Container.Bind<SaveManager>().AsSingle();
    }

    private void Dev()
    {
      // 4. Дев-инструменты (убрать перед релизом, День 25)
      Container.BindInterfacesAndSelfTo<DebugEventLogger>().AsSingle().NonLazy();
    }
  }
}