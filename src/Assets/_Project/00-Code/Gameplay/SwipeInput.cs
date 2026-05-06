using Mergeburgers.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Mergeburgers.Gameplay
{
  /// <summary>
  /// Детектит свайп по экрану через New Input System.
  /// Использует Pointer.current — абстракция над Mouse/Touchscreen,
  /// работает на десктопе и мобильном WebGL без условных веток.
  ///
  /// Логика: запоминаем стартовую позицию при нажатии, считаем дельту при отпускании,
  /// если длина > MinSwipeDistance — определяем направление и публикуем сигнал.
  /// </summary>
  public class SwipeInput : MonoBehaviour
  {
    [Header("Tuning")] 
    [SerializeField] private float _minSwipeDistance = 50f; // в пикселях экрана
    [SerializeField] private float _maxSwipeDuration = 1.0f; // более долгое = drag, не свайп

    private SignalBus _signalBus;

    private Vector2 _startPosition;
    private float _startTime;
    private bool _isTracking;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
      _signalBus = signalBus;
    }

    private void Update()
    {
      var pointer = Pointer.current;
      if (pointer == null) return;

      // Pointer.press — это ButtonControl (true пока кнопка/палец нажаты)
      bool isPressed = pointer.press.isPressed;

      // Начало касания
      if (isPressed && !_isTracking)
      {
        _startPosition = pointer.position.ReadValue();
        _startTime = Time.unscaledTime;
        _isTracking = true;
        return;
      }

      // Конец касания — считаем результат
      if (!isPressed && _isTracking)
      {
        _isTracking = false;

        var endPosition = pointer.position.ReadValue();
        var delta = endPosition - _startPosition;
        var duration = Time.unscaledTime - _startTime;

        if (duration > _maxSwipeDuration)
        {
          // Слишком долго — это drag, не свайп
          return;
        }

        if (delta.magnitude < _minSwipeDistance)
        {
          // Слишком короткий — это тап, не свайп
          return;
        }

        var direction = GetDominantDirection(delta);
        _signalBus.Fire(new SwipeDetectedSignal(direction));
      }
    }

    private static SwipeDirection GetDominantDirection(Vector2 delta)
    {
      // Доминирует горизонтальная или вертикальная ось
      if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
      {
        return delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
      }
      else
      {
        return delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
      }
    }
  }
}