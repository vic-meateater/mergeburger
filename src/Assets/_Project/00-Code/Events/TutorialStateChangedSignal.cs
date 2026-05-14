using Mergeburgers.Tutorial;

namespace Mergeburgers.Events
{
  public sealed class TutorialStateChangedSignal
  {
    public TutorialState NewState { get; }

    public TutorialStateChangedSignal(TutorialState newState)
    {
      NewState = newState;
    }
  }
}