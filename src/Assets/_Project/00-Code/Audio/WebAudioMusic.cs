using System.Runtime.InteropServices;

namespace Mergeburgers.Audio
{
    public static class WebAudioMusic
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void WebAudioMusic_Init();
        [DllImport("__Internal")] private static extern void WebAudioMusic_Load(string url);
        [DllImport("__Internal")] private static extern void WebAudioMusic_Play(bool loop);
        [DllImport("__Internal")] private static extern void WebAudioMusic_Stop();
        [DllImport("__Internal")] private static extern void WebAudioMusic_SetVolume(float vol);
        [DllImport("__Internal")] private static extern void WebAudioMusic_Pause();
        [DllImport("__Internal")] private static extern void WebAudioMusic_Resume();
#else
        private static void WebAudioMusic_Init()
        {
        }

        private static void WebAudioMusic_Load(string url)
        {
        }

        private static void WebAudioMusic_Play(bool loop)
        {
        }

        private static void WebAudioMusic_Stop()
        {
        }

        private static void WebAudioMusic_SetVolume(float vol)
        {
        }

        private static void WebAudioMusic_Pause()
        {
        }

        private static void WebAudioMusic_Resume()
        {
        }
#endif

        public static void Init() => WebAudioMusic_Init();
        public static void Load(string url) => WebAudioMusic_Load(url);
        public static void Play(bool loop = true) => WebAudioMusic_Play(loop);
        public static void Stop() => WebAudioMusic_Stop();
        public static void SetVolume(float vol) => WebAudioMusic_SetVolume(vol);
        public static void Pause() => WebAudioMusic_Pause();
        public static void Resume() => WebAudioMusic_Resume();
    }
}