using System.Runtime.InteropServices;

namespace Mergeburgers.Audio
{
  public static class Vibration
  {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void VibrateJS(int milliseconds);
#else
    private static void VibrateJS(int milliseconds)
    {
    }
#endif

    public static void Light() => VibrateJS(15); // короткая, для свайпа
    public static void Medium() => VibrateJS(40); // для merge / button tap
    public static void Strong() => VibrateJS(80); // для burger sold / event
    public static void Custom(int ms) => VibrateJS(ms);
  }
}