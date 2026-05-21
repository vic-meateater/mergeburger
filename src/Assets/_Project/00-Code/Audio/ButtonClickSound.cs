using Mergeburgers.Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Mergeburgers.Audio
{
  /// <summary>
  /// Вешается на любую UI Button. При нажатии играет ButtonClick из AudioConfig.
  /// </summary>
  [RequireComponent(typeof(Button))]
  public sealed class ButtonClickSound : MonoBehaviour
  {
    private AudioService _audio;
    private Button _button;

    [Inject]
    public void Construct(AudioService audio)
    {
      _audio = audio;
    }

    private void Awake()
    {
      _button = GetComponent<Button>();
    }

    private void Start()
    {
      _button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
      _button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
      _audio.PlayButtonClick();
      Vibration.Light();
    }
  }
}