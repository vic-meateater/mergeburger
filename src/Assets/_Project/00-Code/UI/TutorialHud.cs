using Mergeburgers.Core;
using Mergeburgers.Events;
using Mergeburgers.Tutorial;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Mergeburgers.UI
{
  /// <summary>
  /// UI туториала на сцене Game.
  /// Текстовая плашка с текущей подсказкой + кнопка "Пропустить".
  /// При State.Complete показывает попап поздравления.
  /// </summary>
  public sealed class TutorialHud : MonoBehaviour
  {
    [Header("Hint panel (always visible during tutorial)")] 
    [SerializeField] private GameObject _hintPanel;
    [SerializeField] private GameObject _swipeLabel;
    [SerializeField] private GameObject _mergeLabel;
    [SerializeField] private GameObject _firstBurgerLabel;
    [SerializeField] private Button _skipButton;

    [Header("Completion popup")] 
    [SerializeField] private GameObject _completePopup;

    [SerializeField] private Button _completeCloseButton;

    private SignalBus _signalBus;
    private TutorialController _tutorial;
    private ModalState _modalState;

    private bool _completeBlocking;

    [Inject]
    public void Construct(SignalBus signalBus, TutorialController tutorial, ModalState modalState)
    {
      _signalBus = signalBus;
      _tutorial = tutorial;
      _modalState = modalState;
    }

    private void Start()
    {
      _hintPanel.SetActive(false);
      SetCompletePopup(false);
      HideAllHintLabels();
      
      _skipButton.onClick.AddListener(OnSkip);
      _completeCloseButton.onClick.AddListener(OnCompleteClose);

      _signalBus.Subscribe<TutorialStateChangedSignal>(OnStateChanged);

      _tutorial.EnsureStateInitialized();

      // Применяем текущее состояние сразу
      ApplyState(_tutorial.State);
    }

    private void OnDestroy()
    {
      if (_signalBus != null)
        _signalBus.Unsubscribe<TutorialStateChangedSignal>(OnStateChanged);

      _skipButton.onClick.RemoveListener(OnSkip);
      _completeCloseButton.onClick.RemoveListener(OnCompleteClose);
    }

    private void OnStateChanged(TutorialStateChangedSignal s) => ApplyState(s.NewState);

    private void ApplyState(TutorialState state)
    {
      
      HideAllHintLabels();
      
      switch (state)
      {
        case TutorialState.SwipeAny:
          _hintPanel.SetActive(true);
          _swipeLabel.SetActive(true);
          //_swipeLabel.text = "Свайпни в любую сторону, чтобы передвинуть плитки";
          break;

        case TutorialState.MergeAny:
          _hintPanel.SetActive(true);
          _mergeLabel.SetActive(true);
          //_swipeLabel.text = "Найди две одинаковых плитки рядом — свайпни так, чтобы они столкнулись";
          break;

        case TutorialState.BurgerFirst:
          _hintPanel.SetActive(true);
          _firstBurgerLabel.SetActive(true);
          //_swipeLabel.text = "Собери в ряд: Булка → Котлета → Булка. Получится Бургер!";
          break;

        case TutorialState.Complete:
          _hintPanel.SetActive(false);
          SetCompletePopup(true);
          break;

        case TutorialState.AlreadyPassed:
        case TutorialState.Inactive:
        default:
          _hintPanel.SetActive(false);
          SetCompletePopup(false);
          break;
      }
    }
    
    private void SetCompletePopup(bool active)
    {
      _completePopup.SetActive(active);

      if (active && !_completeBlocking)
      {
        _completeBlocking = true;
        _modalState.Push();
      }
      else if (!active && _completeBlocking)
      {
        _completeBlocking = false;
        _modalState.Pop();
      }
    }

    private void HideAllHintLabels()
    {
      _swipeLabel.SetActive(false);
      _mergeLabel.SetActive(false);
      _firstBurgerLabel.SetActive(false);
    }

    private void OnSkip()
    {
      _tutorial.Skip();
    }

    private void OnCompleteClose()
    {
      SetCompletePopup(false);
      _tutorial.DismissCompletePopup();
    }
  }
}