#if !UNITY_EDITOR
using System.Threading.Tasks;
using YG;

namespace Mergeburgers.Yandex.WebGL
{
    public sealed class WebGLCloudSaveService : ICloudSaveService
    {
        private TaskCompletionSource<bool> _initTcs;

        public WebGLCloudSaveService()
        {
            _initTcs = new TaskCompletionSource<bool>();
            
            if (YG2.isSDKEnabled)
            {
                _initTcs.TrySetResult(true);
            }
            else
            {
                YG2.onGetSDKData += OnSdkReady;
            }
        }
        
        private void OnSdkReady()
        {
            YG2.onGetSDKData -= OnSdkReady;
            _initTcs.TrySetResult(true);
        }
        
        public async Task<string> LoadAsync()
        {
            await _initTcs.Task;
            // Plugin сам подгрузил YG2.saves из облака к моменту onGetSDKData
            return YG2.saves.gameSaveJson;
        }

        public Task SaveAsync(string json)
        {
            YG2.saves.gameSaveJson = json;
            YG2.SaveProgress();
            return Task.CompletedTask;
        }
    }
}
#endif