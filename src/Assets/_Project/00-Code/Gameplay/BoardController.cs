using System;
using Mergeburgers.Events;
using Zenject;

namespace Mergeburgers.Gameplay
{
  public sealed class BoardController : IInitializable, IDisposable
  {
    private readonly SignalBus _signalBus;
    private readonly Board _board;

    public BoardController(SignalBus signalBus, Board board)
    {
      _signalBus = signalBus;
      _board = board;
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
      _board.HandleSwipe(s.Direction);
    }
  }
}