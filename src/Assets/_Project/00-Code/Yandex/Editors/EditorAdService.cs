#if UNITY_EDITOR
using System.Threading.Tasks;
using UnityEngine;

namespace Mergeburgers.Yandex.Editor
{
  public sealed class EditorAdService : IAdService
  {
    public bool IsReady => true;

    public async Task<AdResult> ShowRewardedAsync(string placementId)
    {
      Debug.Log($"[EditorAdService] Rewarded requested: {placementId}");
      await Task.Delay(500);
      Debug.Log($"[EditorAdService] Rewarded completed (mock success)");
      return AdResult.Success;
    }

    public async Task ShowInterstitialAsync(string placementId)
    {
      Debug.Log($"[EditorAdService] Interstitial requested: {placementId}");
      await Task.Delay(300);
      Debug.Log($"[EditorAdService] Interstitial completed (mock)");
    }
  }
}
#endif