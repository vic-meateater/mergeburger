//#if !UNITY_EDITOR

using System;
using System.Threading.Tasks;
using UnityEngine;
using YG;

namespace Mergeburgers.Yandex.WebGL
{
    public sealed class WebGLCloudSaveService : ICloudSaveService
    {
        private TaskCompletionSource<bool> _initTcs;

        // Конструктор пустой — никаких обращений к YG2 здесь.
        // YG2.isSDKEnabled и YG2.onGetSDKData могут бросать exception
        // если плагин ещё не инициализировал JS-мост.
        public WebGLCloudSaveService()
        {
            Debug.Log("[WebGLCloudSave] ctor called");
        }

        public async Task<string> LoadAsync()
        {
            Debug.Log("[WebGLCloudSave] LoadAsync entered");
            await EnsureSdkReadyAsync();
            var json = YG2.saves.gameSaveJson ?? string.Empty;
            Debug.Log($"[WebGLCloudSave] LoadAsync got json length={json.Length}");
            return json;
        }

        public Task SaveAsync(string json)
        {
            YG2.saves.gameSaveJson = json;
            YG2.SaveProgress();
            return Task.CompletedTask;
        }

        private Task EnsureSdkReadyAsync()
        {
            if (_initTcs != null)
                return _initTcs.Task;

            _initTcs = new TaskCompletionSource<bool>();

            try
            {
                if (YG2.isSDKEnabled)
                {
                    Debug.Log("[WebGLCloudSave] SDK already ready");
                    _initTcs.TrySetResult(true);
                }
                else
                {
                    Debug.Log("[WebGLCloudSave] Subscribing to onGetSDKData");
                    YG2.onGetSDKData += OnSdkReady;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[WebGLCloudSave] EnsureSdkReady failed: {e}");
                _initTcs.TrySetResult(false);
            }

            return _initTcs.Task;
        }

        private void OnSdkReady()
        {
            Debug.Log("[WebGLCloudSave] OnSdkReady fired");
            YG2.onGetSDKData -= OnSdkReady;
            _initTcs.TrySetResult(true);
        }
    }
}
//#endif