using Mergeburgers.Events;
using Mergeburgers.Meta;
using Mergeburgers.Tutorial;
using Mergeburgers.Yandex;
using Zenject;

#if UNITY_EDITOR
using Mergeburgers.Yandex.Editor;

#else
using Mergeburgers.Yandex.WebGL;
using Mergeburgers.Yandex.Editor;
#endif

namespace Mergeburgers.Core
{
  public sealed class GameInstaller : MonoInstaller
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
            Container.Bind<ICloudSaveService>().To<WebGLCloudSaveService>().AsSingle();
            Container.Bind<ILeaderboardService>().To<EditorLeaderboardService>().AsSingle();
            Container.Bind<IIAPService>().To<EditorIAPService>().AsSingle();
        }
#endif

    private void Signals()
    {
      SignalBusInstaller.Install(Container);
      
      Container.DeclareSignal<GameOverSignal>();
      Container.DeclareSignal<BurgerCreatedSignal>();
      Container.DeclareSignal<EnergySpentSignal>();
      Container.DeclareSignal<LevelMilestoneSignal>();
      Container.DeclareSignal<SwipeDetectedSignal>();
      Container.DeclareSignal<BurgerSoldSignal>();
      Container.DeclareSignal<SessionCoinsChangedSignal>();
      Container.DeclareSignal<BankCoinsChangedSignal>();
      Container.DeclareSignal<EnergyChangedSignal>();
      Container.DeclareSignal<CashedOutSignal>();
      Container.DeclareSignal<MergeOccurredSignal>().OptionalSubscriber();
      Container.DeclareSignal<TutorialStateChangedSignal>();


      //Container.DeclareSignal<CoinsChangedSignal>();
    }

    private void Managers()
    {
      Container.Bind<SaveManager>().AsSingle();
      Container.Bind<IdleIncomeCalculator>().AsSingle();
      Container.Bind<EnergyManager>().AsSingle();
      Container.BindInterfacesAndSelfTo<InterstitialDispatcher>().AsSingle().NonLazy();
      Container.BindInterfacesAndSelfTo<AutosaveDispatcher>().AsSingle().NonLazy();
      Container.BindInterfacesAndSelfTo<EconomyManager>().AsSingle().NonLazy();
      Container.BindInterfacesAndSelfTo<RecipeUnlockTracker>().AsSingle().NonLazy();
      Container.BindInterfacesAndSelfTo<TutorialController>().AsSingle().NonLazy();

    }

    private void Dev()
    {
      Container.BindInterfacesAndSelfTo<DebugEventLogger>().AsSingle().NonLazy();
    }
  }
}