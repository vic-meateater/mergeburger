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
    [Header("Hint panel (always visible during tutorial)")] [SerializeField]
    private GameObject _hintPanel;

    [SerializeField] private TextMeshProUGUI _hintLabel;
    [SerializeField] private Button _skipButton;

    [Header("Completion popup")] [SerializeField]
    private GameObject _completePopup;

    [SerializeField] private Button _completeCloseButton;

    private SignalBus _signalBus;
    private TutorialController _tutorial;

    [Inject]
    public void Construct(SignalBus signalBus, TutorialController tutorial)
    {
      _signalBus = signalBus;
      _tutorial = tutorial;
    }

    private void Start()
    {
      _hintPanel.SetActive(false);
      _completePopup.SetActive(false);
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
      switch (state)
      {
        case TutorialState.SwipeAny:
          _hintPanel.SetActive(true);
          _hintLabel.text = "Свайпни в любую сторону, чтобы передвинуть плитки";
          break;

        case TutorialState.MergeAny:
          _hintPanel.SetActive(true);
          _hintLabel.text = "Найди две одинаковых плитки рядом — свайпни так, чтобы они столкнулись";
          break;

        case TutorialState.BurgerFirst:
          _hintPanel.SetActive(true);
          _hintLabel.text = "Собери в ряд: Булка → Котлета → Булка. Получится Бургер!";
          break;

        case TutorialState.Complete:
          _hintPanel.SetActive(false);
          _completePopup.SetActive(true);
          break;

        case TutorialState.AlreadyPassed:
        case TutorialState.Inactive:
        default:
          _hintPanel.SetActive(false);
          _completePopup.SetActive(false);
          break;
      }
    }

    private void OnSkip()
    {
      _tutorial.Skip();
    }

    private void OnCompleteClose()
    {
      _completePopup.SetActive(false);
    }
  }
}