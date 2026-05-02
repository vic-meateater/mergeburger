using System.Threading.Tasks;
using UnityEngine;

namespace Mergeburgers.Yandex.Editor
{
  public class EditorIAPService : IIAPService
  {
    public bool IsAvailable => true;

    public async Task<bool> PurchaseAsync(string productId)
    {
      Debug.Log($"[EditorIAP] Purchase requested: {productId}");
      await Task.Delay(500);
      Debug.Log($"[EditorIAP] Purchase completed (mock success)");
      return true;
    }
  }
}