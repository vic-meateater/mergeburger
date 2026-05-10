using Mergeburgers.Core;
using TMPro;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Meta
{
  public sealed class IdleRateLabel : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _label;

    private SaveManager _saveManager;
    private IdleIncomeCalculator _calculator;

    [Inject]
    public void Construct(SaveManager saveManager, IdleIncomeCalculator calculator)
    {
      _saveManager = saveManager;
      _calculator = calculator;
    }

    private void Update()
    {
      int rate = _calculator.CalculateRate(_saveManager.Current);
      _label.text = rate > 0 ? $"+{rate}/сек" : "Соберите бургер чтобы открыть пассивный доход";
    }
  }
}