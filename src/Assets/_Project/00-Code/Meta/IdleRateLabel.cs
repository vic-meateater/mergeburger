using Mergeburgers.Core;
using TMPro;
using UnityEngine;
using Zenject;

namespace Mergeburgers.Meta
{
  public sealed class IdleRateLabel : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private TextMeshProUGUI _tutorialLabel;

    private SaveManager _saveManager;
    private IdleIncomeCalculator _calculator;

    [Inject]
    public void Construct(SaveManager saveManager, IdleIncomeCalculator calculator)
    {
      _saveManager = saveManager;
      _calculator = calculator;
    }

    private void OnEnable()
    {
      int rate = _calculator.CalculateRate(_saveManager.Current);
      if(rate > 0)
        _tutorialLabel.enabled = false;
    }

    private void Update()
    {
      int rate = _calculator.CalculateRate(_saveManager.Current);
      _label.text = rate > 0 ? $"+{rate}" : "0";
    }
  }
}