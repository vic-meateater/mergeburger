using Zenject;
#if UNITY_WEBGL && !UNITY_EDITOR
using YG;
#endif

namespace Mergeburgers.Audio
{
    public sealed class AdAudioMuter : IInitializable, System.IDisposable
    {
        private readonly AudioService _audio;

        [Inject]
        public AdAudioMuter(AudioService audio) => _audio = audio;

        public void Initialize()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // onOpenAnyAdv/onCloseAnyAdv/onErrorAnyAdv вызываются и для interstitial, и для rewarded.
            YG2.onOpenAnyAdv  += _audio.PauseAll;
            YG2.onCloseAnyAdv += _audio.ResumeAll;
            YG2.onErrorAnyAdv += _audio.ResumeAll;   // при сбое рекламы вернуть звук, иначе тишина навсегда
#endif
        }

        public void Dispose()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            YG2.onOpenAnyAdv  -= _audio.PauseAll;
            YG2.onCloseAnyAdv -= _audio.ResumeAll;
            YG2.onErrorAnyAdv -= _audio.ResumeAll;
#endif
        }
    }
}
