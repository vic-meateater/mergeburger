using UnityEngine;

namespace Mergeburgers.Audio
{
  [CreateAssetMenu(fileName = "AudioConfig", menuName = "Mergeburgers/Audio Config", order = 0)]
  public sealed class AudioConfig : ScriptableObject
  {
    [Header("Volume")] 
    [Range(0f, 1f)] public float SfxVolume = 0.7f;
    [Range(0f, 1f)] public float MusicVolume = 0.4f;

    [Header("SFX Clips (used as filename in StreamingAssets/sfx)")]
    public AudioClip Swipe;
    public AudioClip Merge;
    public AudioClip BurgerCreated;
    public AudioClip BurgerSold;
    public AudioClip ButtonClick;

    [Header("Music (StreamingAssets/music)")]
    public AudioClip MenuMusic;
    public AudioClip GameMusic;
  }
}