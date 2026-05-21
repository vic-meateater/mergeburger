using Mergeburgers.Events;
using Zenject;

namespace Mergeburgers.Audio
{
  /// <summary>
  /// Подписывается на геймплей-сигналы и проигрывает соответствующие звуки и вибрации.
  /// IInitializable/IDisposable lifecycle через Zenject.
  /// </summary>
  public sealed class AudioSignalListener : IInitializable, System.IDisposable
  {
    private readonly SignalBus _signalBus;
    private readonly AudioService _audio;

    [Inject]
    public AudioSignalListener(SignalBus signalBus, AudioService audio)
    {
      _signalBus = signalBus;
      _audio = audio;
    }

    public void Initialize()
    {
      _signalBus.Subscribe<SwipeDetectedSignal>(OnSwipe);
      _signalBus.Subscribe<MergeOccurredSignal>(OnMerge);
      _signalBus.Subscribe<BurgerCreatedSignal>(OnBurgerCreated);
      _signalBus.Subscribe<BurgerSoldSignal>(OnBurgerSold);
    }

    public void Dispose()
    {
      _signalBus.TryUnsubscribe<SwipeDetectedSignal>(OnSwipe);
      _signalBus.TryUnsubscribe<MergeOccurredSignal>(OnMerge);
      _signalBus.TryUnsubscribe<BurgerCreatedSignal>(OnBurgerCreated);
      _signalBus.TryUnsubscribe<BurgerSoldSignal>(OnBurgerSold);
    }

    private void OnSwipe(SwipeDetectedSignal s)
    {
      _audio.PlaySwipe();
      Vibration.Light();
    }

    private void OnMerge(MergeOccurredSignal s)
    {
      _audio.PlayMerge();
      Vibration.Medium();
    }

    private void OnBurgerCreated(BurgerCreatedSignal s)
    {
      _audio.PlayBurgerCreated();
      Vibration.Strong();
    }

    private void OnBurgerSold(BurgerSoldSignal s)
    {
      _audio.PlayBurgerSold();
    }
  }
}