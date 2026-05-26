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
      return json;
    }

    public async Task SaveAsync(string json)
    {
      await Task.Delay(100);
      PlayerPrefs.SetString(KEY, json);
      PlayerPrefs.Save();
    }
  }
}

#endif