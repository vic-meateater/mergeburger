using Mergeburgers.Events;
using Mergeburgers.Meta;
using Mergeburgers.Yandex;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Mergeburgers.UI
{
  public sealed class EnergyHud : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private TextMeshProUGUI _timerLabel;
    [SerializeField] private Button _plusButton;

    private EnergyManager _energy;
    private SignalBus _signalBus;
    private IAdService _adService;

    [Inject]
    public void Construct(
      EnergyManager energy,
      SignalBus signalBus,
      IAdService adService)
    {
      _energy = energy;
      _signalBus = signalBus;
      _adService = adService;
    }

    private void Start()
    {
      UpdateLabels();
      _signalBus.Subscribe<EnergyChangedSignal>(OnEnergyChanged);
      if (_plusButton != null)
        _plusButton.onClick.AddListener(OnPlusClicked);
    }

    private void OnDestroy()
    {
      if (_signalBus != null)
        _signalBus.Unsubscribe<EnergyChangedSignal>(OnEnergyChanged);
      if (_plusButton != null)
        _plusButton.onClick.RemoveListener(OnPlusClicked);
    }

    private void Update()
    {
      // Раз в секунду обновлять таймер — энергия может тикнуть сама
      UpdateTimer();
    }

    private void OnEnergyChanged(EnergyChangedSignal s)
    {
      UpdateLabels();
    }

    private void UpdateLabels()
    {
      _label.text = $"{_energy.Current}/{_energy.Max}";
      UpdateTimer();
      UpdatePlusButton();
    }

    private void UpdateTimer()
    {
      if (_energy.IsFull)
      {
        _timerLabel.text = "ПОЛНО";
        return;
      }

      int seconds = _energy.SecondsUntilNext;
      int mm = seconds / 60;
      int ss = seconds % 60;
      _timerLabel.text = $"{mm:00}:{ss:00}";

      // Force update Current (триггерит regenerate) — может изменить энергию,
      // ChangeSignal сам обновит лейблы
      var _ = _energy.Current;
    }

    private void UpdatePlusButton()
    {
      if (_plusButton == null) return;
      _plusButton.interactable = !_energy.IsFull;
    }

    private async void OnPlusClicked()
    {
      _plusButton.interactable = false;
      var result = await _adService.ShowRewardedAsync(EnergyConfig.RewardedPlacementId);
      _plusButton.interactable = !_energy.IsFull;

      if (result == Yandex.AdResult.Success)
        _energy.Add(1);
    }
  }
}