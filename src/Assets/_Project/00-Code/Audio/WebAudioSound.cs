using System.Runtime.InteropServices;

namespace Game.Code.Audio
{
    public static class WebAudioSound
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void WebAudioSound_Init();
        [DllImport("__Internal")] private static extern void WebAudioSound_Load(string key, string url);
        [DllImport("__Internal")] private static extern void WebAudioSound_Play(string key, float volume);
#else
        private static void WebAudioSound_Init()
        {
        }

        private static void WebAudioSound_Load(string key, string url)
        {
        }

        private static void WebAudioSound_Play(string key, float volume)
        {
        }
#endif

        public static void Init() => WebAudioSound_Init();
        public static void Load(string key, string url) => WebAudioSound_Load(key, url);
        public static void Play(string key, float volume = 1f) => WebAudioSound_Play(key, volume);
    }
}