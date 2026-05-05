#if !UNITY_EDITOR

using System;
using System.Threading.Tasks;
using YG;

namespace Mergeburgers.Yandex.WebGL
{
    public class WebGLAdService  : IAdService
    {
        public bool IsReady => YG2.isSDKEnabled;

        public Task<AdResult> ShowRewardedAsync(string placementId)
        {
            var tcs = new TaskCompletionSource<AdResult>();
            bool rewarded = false;

            Action onReward = () => { rewarded = true; };
            Action onClose = null;
            onClose = () =>
            {
                YG2.onCloseAnyAdv -= onClose;
                tcs.TrySetResult(rewarded ? AdResult.Success : AdResult.Skipped);
            };

            YG2.onCloseAnyAdv += onClose;
            YG2.RewardedAdvShow(placementId, onReward);

            return tcs.Task;
        }

        public Task ShowInterstitialAsync(string placementId)
        {
            var tcs = new TaskCompletionSource<bool>();
            Action<bool> onClose = null;
            onClose = wasShown =>
            {
                YG2.onCloseInterAdvWasShow -= onClose;
                tcs.TrySetResult(wasShown);
            };
            YG2.onCloseInterAdvWasShow += onClose;
            YG2.InterstitialAdvShow();
            return tcs.Task;
        }
    }
}
#endif