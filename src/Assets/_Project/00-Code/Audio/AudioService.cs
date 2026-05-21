using UnityEngine;

namespace Mergeburgers.Audio
{
  public class AudioService
  {
    private readonly AudioConfig _config;
    private bool _isSfxMuted;

#if UNITY_WEBGL && !UNITY_EDITOR
        private string _currentMusicKey;
        private bool _musicMuted;
        private float _musicVolume;
#else
    private readonly AudioSource _sfxSource;
    private readonly AudioSource _musicSource;
#endif

    public float SfxVolume => _config.SfxVolume;
    public bool IsSfxMuted => _isSfxMuted;

#if UNITY_WEBGL && !UNITY_EDITOR
        public float MusicVolume => _musicVolume;
        public bool IsMusicMuted => _musicMuted;
#else
    public float MusicVolume => _musicSource.volume;
    public bool IsMusicMuted => _musicSource.mute;
#endif

    public AudioService(AudioConfig config)
    {
      _config = config;

#if UNITY_WEBGL && !UNITY_EDITOR
            _musicVolume = _config.MusicVolume;

            WebAudioMusic.Init();
            WebAudioMusic.SetVolume(_musicVolume);

            WebAudioSound.Init();
            LoadAllSfx();
#else
      var audioObject = new GameObject("[AudioService]");
      Object.DontDestroyOnLoad(audioObject);

      _sfxSource = audioObject.AddComponent<AudioSource>();
      _sfxSource.playOnAwake = false;

      _musicSource = audioObject.AddComponent<AudioSource>();
      _musicSource.playOnAwake = false;
      _musicSource.loop = true;
      _musicSource.volume = _config.MusicVolume;
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
        private void LoadAllSfx()
        {
            var path = Application.streamingAssetsPath + "/sfx/";

            LoadClip(_config.Swipe, path);
            LoadClip(_config.Merge, path);
            LoadClip(_config.BurgerCreated, path);
            LoadClip(_config.BurgerSold, path);
            LoadClip(_config.ButtonClick, path);
        }

        private void LoadClip(AudioClip clip, string basePath)
        {
            if (clip != null)
                WebAudioSound.Load(clip.name, basePath + clip.name + ".ogg");
        }
#endif

    // --- Music ---

    public void PlayMenuMusic() => PlayMusic(_config.MenuMusic);
    public void PlayGameMusic() => PlayMusic(_config.GameMusic);

    private void PlayMusic(AudioClip clip)
    {
      if (clip == null) return;
#if UNITY_WEBGL && !UNITY_EDITOR
            var key = clip.name;
            if (_currentMusicKey == key) return;
            _currentMusicKey = key;
            WebAudioMusic.Stop();
            WebAudioMusic.Load(Application.streamingAssetsPath + "/music/" + key + ".ogg");
            WebAudioMusic.Play(loop: true);
#else
      if (_musicSource.clip != clip)
      {
        _musicSource.clip = clip;
        _musicSource.Play();
      }
#endif
    }

    public void StopMusic()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebAudioMusic.Stop();
            _currentMusicKey = null;
#else
      _musicSource.Stop();
#endif
    }

    public void TryResumeMusic()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebAudioMusic.Resume();
#else
      if (_musicSource.clip != null && !_musicSource.isPlaying)
        _musicSource.Play();
#endif
    }

    // --- SFX ---

    public void PlaySwipe() => PlaySfx(_config.Swipe);
    public void PlayMerge() => PlaySfx(_config.Merge);
    public void PlayBurgerCreated() => PlaySfx(_config.BurgerCreated);
    public void PlayBurgerSold() => PlaySfx(_config.BurgerSold);
    public void PlayButtonClick() => PlaySfx(_config.ButtonClick);

    private void PlaySfx(AudioClip clip)
    {
      if (clip == null || _isSfxMuted) return;
#if UNITY_WEBGL && !UNITY_EDITOR
            WebAudioSound.Play(clip.name, _config.SfxVolume);
#else
      _sfxSource.PlayOneShot(clip, _config.SfxVolume);
#endif
    }

    // --- Volume / Mute ---

    public void MuteSfx(bool mute)
    {
      _isSfxMuted = mute;
#if !UNITY_WEBGL || UNITY_EDITOR
      _sfxSource.mute = mute;
#endif
    }

    public void MuteMusic(bool mute)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            _musicMuted = mute;
            WebAudioMusic.SetVolume(mute ? 0f : _musicVolume);
#else
      _musicSource.mute = mute;
#endif
    }

    public void SetSfxVolume(float value) => _config.SfxVolume = Mathf.Clamp01(value);

    public void SetMusicVolume(float value)
    {
      value = Mathf.Clamp01(value);
#if UNITY_WEBGL && !UNITY_EDITOR
            _musicVolume = value;
            if (!_musicMuted) WebAudioMusic.SetVolume(value);
#else
      _musicSource.volume = value;
#endif
    }
  }
}