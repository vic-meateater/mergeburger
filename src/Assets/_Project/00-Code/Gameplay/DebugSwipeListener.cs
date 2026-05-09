using System;
using Mergeburgers.Events;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Gameplay
{
  public sealed class DebugSwipeListener : IInitializable, IDisposable
  {
    private readonly SignalBus _signalBus;
    private SwipeDirection _swipeDirection;
    
    public SwipeDirection SwipeDirection => _swipeDirection;

    [Inject]
    public DebugSwipeListener(SignalBus signalBus)
    {
      _signalBus = signalBus;
    }

    public void Initialize()
    {
      _signalBus.Subscribe<SwipeDetectedSignal>(OnSwipe);
    }

    public void Dispose()
    {
      _signalBus.Unsubscribe<SwipeDetectedSignal>(OnSwipe);
    }

    private void OnSwipe(SwipeDetectedSignal s)
    {
      //Debug.Log($"[Input] Swipe {s.Direction}");
      _swipeDirection = s.Direction;
    }
  }
}