using UnityEngine;

public class AudioManager
{
    private readonly AudioConfig _config;
    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    private bool _musicEnabled = true;
    private bool _sfxEnabled = true;

    private const string MusicEnabledKey = "MusicEnabled";
    private const string SFXEnabledKey = "SFXEnabled";

    public bool MusicEnabled => _musicEnabled;
    public bool SFXEnabled => _sfxEnabled;

    public AudioManager(AudioConfig config)
    {
        _config = config;

        _musicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
        _sfxEnabled = PlayerPrefs.GetInt(SFXEnabledKey, 1) == 1;

        CreateAudioSources();
    }

    private void CreateAudioSources()
    {
        var go = new GameObject("AudioSources");
        Object.DontDestroyOnLoad(go);

        _musicSource = go.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;
        _musicSource.volume = _musicEnabled ? _config.MusicVolume : 0f;

        _sfxSource = go.AddComponent<AudioSource>();
        _sfxSource.loop = false;
        _sfxSource.playOnAwake = false;
    }

    // === Music ===

    public void PlayMenuMusic()
    {
        PlayMusic(_config.MenuMusic);
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(_config.GameplayMusic);
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (_musicSource.clip == clip && _musicSource.isPlaying) return;

        _musicSource.clip = clip;
        _musicSource.volume = _musicEnabled ? _config.MusicVolume : 0f;
        _musicSource.Play();
    }

    // === SFX ===

    public void PlayButtonClick() => PlaySFX(_config.ButtonClick);
    public void PlayPopupOpen() => PlaySFX(_config.PopupOpen);
    public void PlayPopupClose() => PlaySFX(_config.PopupClose);
    public void PlayPiecePickup() => PlaySFX(_config.PiecePickup);
    public void PlayPiecePlace() => PlaySFX(_config.PiecePlace);
    public void PlayPieceReturn() => PlaySFX(_config.PieceReturn);
    private int _chainCount;
    private const float ChainPitchStep = 0.15f;
    private const float ChainPitchMin = 1f;
    private const float ChainPitchMax = 2f;

    public void PlayMerge()
    {
        _chainCount = 0;
        PlaySFX(_config.Merge);
    }

    public void PlayChainMerge()
    {
        _chainCount++;
        float pitch = Mathf.Clamp(ChainPitchMin + _chainCount * ChainPitchStep, ChainPitchMin, ChainPitchMax);
        PlaySFXWithPitch(_config.Merge, pitch);
    }
    public void PlayLineClear() => PlaySFX(_config.LineClear);
    public void PlayScoreUp() => PlaySFX(_config.ScoreUp);
    public void PlayGameOver() => PlaySFX(_config.GameOver);
    public void PlayGameStart() => PlaySFX(_config.GameStart);
    public void PlayNewPiecesSpawn() => PlaySFX(_config.NewPiecesSpawn);
    public void PlayTutorialStep() => PlaySFX(_config.TutorialStep);
    public void PlayTutorialComplete() => PlaySFX(_config.TutorialComplete);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || !_sfxEnabled) return;
        _sfxSource.pitch = 1f;
        _sfxSource.PlayOneShot(clip, _config.SFXVolume);
    }

    private void PlaySFXWithPitch(AudioClip clip, float pitch)
    {
        if (clip == null || !_sfxEnabled) return;
        _sfxSource.pitch = pitch;
        _sfxSource.PlayOneShot(clip, _config.SFXVolume);
    }

    // === Settings ===

    public void ToggleMusic()
    {
        _musicEnabled = !_musicEnabled;
        _musicSource.volume = _musicEnabled ? _config.MusicVolume : 0f;
        PlayerPrefs.SetInt(MusicEnabledKey, _musicEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSFX()
    {
        _sfxEnabled = !_sfxEnabled;
        PlayerPrefs.SetInt(SFXEnabledKey, _sfxEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
