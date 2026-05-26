using Mergeburgers.Core;
using Mergeburgers.Events;
using Zenject;

namespace Mergeburgers.Tutorial
{
  /// <summary>
  /// Управляет логикой состояний туториала. Не MonoBehaviour — plain класс в DI.
  /// UI слушает события через сигнал TutorialStateChangedSignal.
  /// </summary>
  public sealed class TutorialController : IInitializable, System.IDisposable
  {
    private readonly SignalBus _signalBus;
    private readonly SaveManager _saveManager;
    private readonly EconomyManager _economy;
    private bool _stateInitialized;

    public TutorialState State { get; private set; } = TutorialState.Inactive;

    [Inject]
    public TutorialController(SignalBus signalBus, SaveManager saveManager, EconomyManager economy)
    {
      _economy = economy;
      _signalBus = signalBus;
      _saveManager = saveManager;
    }

    public void Initialize()
    {
      _signalBus.Subscribe<SwipeDetectedSignal>(OnSwipe);
      _signalBus.Subscribe<MergeOccurredSignal>(OnMerge);
      _signalBus.Subscribe<BurgerCreatedSignal>(OnBurgerCreated);
    }

    public void EnsureStateInitialized()
    {
      if (_stateInitialized) return;
      _stateInitialized = true;

      bool passed = _saveManager.Current?.tutorialPassed ?? false;

      if (!passed)
        State = TutorialState.SwipeAny;
      else
        State = TutorialState.AlreadyPassed;

      _signalBus.Fire(new TutorialStateChangedSignal(State));
    }


    public void Skip()
    {
      if (State == TutorialState.Inactive || State == TutorialState.Complete) return;
      MarkComplete("skipped");
    }


    public void Dispose()
    {
      _signalBus.TryUnsubscribe<SwipeDetectedSignal>(OnSwipe);
      _signalBus.TryUnsubscribe<MergeOccurredSignal>(OnMerge);
      _signalBus.TryUnsubscribe<BurgerCreatedSignal>(OnBurgerCreated);
    }

    public void DismissCompletePopup()
    {
      if (State == TutorialState.Complete)
      {
        State = TutorialState.AlreadyPassed;
        _signalBus.Fire(new TutorialStateChangedSignal(State));
      }
    }

    private void OnSwipe(SwipeDetectedSignal s)
    {
      EnsureStateInitialized();

      if (State == TutorialState.SwipeAny)
        Advance(TutorialState.MergeAny);
    }

    private void OnMerge(MergeOccurredSignal s)
    {
      EnsureStateInitialized();

      if (State == TutorialState.MergeAny)
        Advance(TutorialState.BurgerFirst);
    }

    private void OnBurgerCreated(BurgerCreatedSignal s)
    {
      EnsureStateInitialized();

      if (State == TutorialState.BurgerFirst)
      {
        Advance(TutorialState.Complete);
        MarkComplete("first_burger_created");
      }
    }

    private void Advance(TutorialState next)
    {
      State = next;
      _signalBus.Fire(new TutorialStateChangedSignal(State));
    }

    private void MarkComplete(string reason)
    {
      if (_saveManager.Current != null)
        _saveManager.Current.tutorialPassed = true;

      if (reason == "first_burger_created")
      {
        _economy.AddSessionCoins(50);
      }

      State = TutorialState.Complete;
      _signalBus.Fire(new TutorialStateChangedSignal(State));

      // Сразу переходим в AlreadyPassed чтобы повторный заход не показал popup
      State = TutorialState.AlreadyPassed;
    }
  }
}