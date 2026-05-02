using System.Threading.Tasks;

namespace Mergeburgers.Yandex
{
  public interface IIAPService
  {
    bool IsAvailable { get; }
    Task<bool> PurchaseAsync(string productId);
  }
}