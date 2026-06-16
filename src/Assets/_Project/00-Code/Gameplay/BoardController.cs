using System;
using Mergeburgers.Core;
using Mergeburgers.Events;
using Zenject;

namespace Mergeburgers.Gameplay
{
  public sealed class BoardController : IInitializable, IDisposable
  {
    private readonly SignalBus _signalBus;
    private readonly Board _board;
    private readonly ModalState _modalState;

    public BoardController(SignalBus signalBus, Board board, ModalState modalState)
    {
      _signalBus = signalBus;
      _board = board;
      _modalState = modalState;
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
      if (_modalState.IsAnyOpen) return;
      _board.HandleSwipe(s.Direction);
    }
  }
}