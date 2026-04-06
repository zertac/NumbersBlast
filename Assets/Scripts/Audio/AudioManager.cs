using UnityEngine;
using NumbersBlast.Core;
using NumbersBlast.Data;

namespace NumbersBlast.Audio
{
    /// <summary>
    /// Manages all game audio including music playback, SFX, and user sound preferences.
    /// </summary>
    public class AudioManager
    {
        // Static instance for MonoBehaviour access (UIButton, SettingsPopup) where DI injection is not available.
        // All pure C# classes should receive AudioManager via constructor injection.
        private static AudioManager _instance;
        private static GameObject _audioGo;

        private readonly AudioConfig _config;
        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        private bool _musicEnabled = true;
        private bool _sfxEnabled = true;


        /// <summary>
        /// Whether background music is currently enabled.
        /// </summary>
        public bool MusicEnabled => _musicEnabled;

        /// <summary>
        /// Whether sound effects are currently enabled.
        /// </summary>
        public bool SFXEnabled => _sfxEnabled;

        public AudioManager(AudioConfig config)
        {
            if (config == null)
            {
#if UNITY_EDITOR || DEBUG
                Debug.LogError("[AudioManager] AudioConfig is null!");
#endif
                return;
            }
            _config = config;

            _musicEnabled = PlayerPrefs.GetInt(GameConstants.MusicEnabledKey, 1) == 1;
            _sfxEnabled = PlayerPrefs.GetInt(GameConstants.SFXEnabledKey, 1) == 1;

            if (_audioGo == null)
            {
                CreateAudioSources();
            }
            else
            {
                // Reuse existing sources
                var sources = _audioGo.GetComponents<AudioSource>();
                _musicSource = sources[0];
                _sfxSource = sources[1];
            }

            _instance = this;
        }

        /// <summary>
        /// Static singleton accessor for contexts where DI injection is not available (e.g. UIButton, SettingsPopup).
        /// </summary>
        public static AudioManager Instance => _instance;

        // One-time runtime creation; persists across scenes via DontDestroyOnLoad.
        private void CreateAudioSources()
        {
            _audioGo = new GameObject("AudioSources");
            Object.DontDestroyOnLoad(_audioGo);

            _musicSource = _audioGo.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.volume = _musicEnabled ? _config.MusicVolume : 0f;

            _sfxSource = _audioGo.AddComponent<AudioSource>();
            _sfxSource.loop = false;
            _sfxSource.playOnAwake = false;
        }

        // === Music ===

        /// <summary>
        /// Starts playing the main menu background music.
        /// </summary>
        public void PlayMenuMusic()
        {
            PlayMusic(_config.MenuMusic);
        }

        /// <summary>
        /// Starts playing the gameplay background music.
        /// </summary>
        public void PlayGameplayMusic()
        {
            PlayMusic(_config.GameplayMusic);
        }

        /// <summary>
        /// Stops the currently playing background music.
        /// </summary>
        public void StopMusic()
        {
            if (_musicSource != null)
                _musicSource.Stop();
        }

        private void PlayMusic(AudioClip clip)
        {
            if (clip == null || _musicSource == null) return;
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
        public void PlayLineClear() => PlaySFX(_config.LineClear);
        public void PlayScoreUp() => PlaySFX(_config.ScoreUp);
        public void PlayGameOver() => PlaySFX(_config.GameOver);
        public void PlayGameStart() => PlaySFX(_config.GameStart);
        public void PlayNewPiecesSpawn() => PlaySFX(_config.NewPiecesSpawn);
        public void PlayTutorialStep() => PlaySFX(_config.TutorialStep);
        public void PlayTutorialComplete() => PlaySFX(_config.TutorialComplete);

        private int _chainCount;
        private const float ChainPitchStep = 0.15f;
        private const float ChainPitchMin = 1f;
        private const float ChainPitchMax = 2f;

        /// <summary>
        /// Plays the merge SFX and resets the chain pitch counter.
        /// </summary>
        public void PlayMerge()
        {
            _chainCount = 0;
            PlaySFX(_config.Merge);
        }

        /// <summary>
        /// Plays the merge SFX with progressively increasing pitch for chain combos.
        /// </summary>
        public void PlayChainMerge()
        {
            _chainCount++;
            float pitch = Mathf.Clamp(ChainPitchMin + _chainCount * ChainPitchStep, ChainPitchMin, ChainPitchMax);
            PlaySFXWithPitch(_config.Merge, pitch);
        }

        private void PlaySFX(AudioClip clip)
        {
            if (clip == null || !_sfxEnabled || _sfxSource == null) return;
            _sfxSource.pitch = 1f;
            _sfxSource.PlayOneShot(clip, _config.SFXVolume);
        }

        private void PlaySFXWithPitch(AudioClip clip, float pitch)
        {
            if (clip == null || !_sfxEnabled || _sfxSource == null) return;
            _sfxSource.pitch = pitch;
            _sfxSource.PlayOneShot(clip, _config.SFXVolume);
        }

        // === Settings ===

        /// <summary>
        /// Toggles music on/off and persists the preference to PlayerPrefs.
        /// </summary>
        public void ToggleMusic()
        {
            _musicEnabled = !_musicEnabled;
            if (_musicSource != null)
                _musicSource.volume = _musicEnabled ? _config.MusicVolume : 0f;
            PlayerPrefs.SetInt(GameConstants.MusicEnabledKey, _musicEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Toggles sound effects on/off and persists the preference to PlayerPrefs.
        /// </summary>
        public void ToggleSFX()
        {
            _sfxEnabled = !_sfxEnabled;
            PlayerPrefs.SetInt(GameConstants.SFXEnabledKey, _sfxEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
