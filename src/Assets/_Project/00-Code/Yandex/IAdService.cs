using System.Threading.Tasks;

namespace Mergeburgers.Yandex
{
  public enum AdResult
  {
    Unknown = 0,
    Success = 1,
    Skipped = 2,
    Failed = 3,
  }
  public interface IAdService
  {
    bool IsReady { get; }
    Task<AdResult> ShowRewardedAsync(string placementId);
    Task ShowInterstitialAsync(string placementId);
  }
}