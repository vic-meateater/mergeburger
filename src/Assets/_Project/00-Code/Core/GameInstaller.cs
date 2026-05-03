using Mergeburgers.Events;
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
      YandexServicesEditor();
#else
            // TODO День 6: WebGL-имплементации через max-games плагин
#endif
      Signals();
      Dev();
    }


    private void YandexServicesEditor()
    {
      Container.Bind<IAdService>().To<EditorAdService>().AsSingle();
      Container.Bind<ICloudSaveService>().To<EditorCloudSaveService>().AsSingle();
      Container.Bind<ILeaderboardService>().To<EditorLeaderboardService>().AsSingle();
      Container.Bind<IIAPService>().To<EditorIAPService>().AsSingle();
    }

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

    private void Dev()
    {
      // 4. Дев-инструменты (убрать перед релизом, День 25)
      Container.BindInterfacesAndSelfTo<DebugEventLogger>().AsSingle().NonLazy();
    }
  }
}