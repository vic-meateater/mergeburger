#if UNITY_EDITOR
using System.Threading.Tasks;
using UnityEngine;

namespace Mergeburgers.Yandex.Editor
{
  public sealed class EditorCloudSaveService : ICloudSaveService
  {
    private const string KEY = "burgermaster_save";
    
    public async Task<string> LoadAsync()
    {
      await Task.Delay(100);
      var json = PlayerPrefs.GetString(KEY, string.Empty);
      Debug.Log($"[EditorCloudSave] Loaded: {(string.IsNullOrEmpty(json) ? "<empty>" : json.Length + " chars")}");
      return json;
    }

    public async Task SaveAsync(string json)
    {
      await Task.Delay(100);
      PlayerPrefs.SetString(KEY, json);
      PlayerPrefs.Save();
      Debug.Log($"[EditorCloudSave] Saved: {json.Length} chars");
    }
  }
}
#endif