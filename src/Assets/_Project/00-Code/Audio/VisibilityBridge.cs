using System.Runtime.InteropServices;

namespace Mergeburgers.Audio
{
    public static class VisibilityBridge
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void RegisterVisibilityBridge();
        [DllImport("__Internal")] private static extern void AudioGate_Pause(string reason);
        [DllImport("__Internal")] private static extern void AudioGate_Resume(string reason);

        public static void Register() => RegisterVisibilityBridge();
        public static void Pause(string reason) => AudioGate_Pause(reason);
        public static void Resume(string reason) => AudioGate_Resume(reason);
#else
        public static void Register() { }
        public static void Pause(string reason) { }
        public static void Resume(string reason) { }
#endif
    }
}
