using UnityEngine;
using Zenject;

namespace Mergeburgers.Gameplay
{
  public class DevLogSwipe : MonoBehaviour
  {
    [SerializeField] private TMPro.TextMeshProUGUI _text;
    
    private DebugSwipeListener _listener;

    [Inject]
    public void Construct(DebugSwipeListener listener)
    {
      _listener = listener;
    }

    private void Update()
    {
      _text.text = _listener.SwipeDirection.ToString();
    }
  }
}