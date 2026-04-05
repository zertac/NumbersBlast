using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPopup : BasePopup
{
    [SerializeField] private Button _musicToggle;
    [SerializeField] private Button _sfxToggle;
    [SerializeField] private Button _hapticToggle;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _musicLabel;
    [SerializeField] private TextMeshProUGUI _sfxLabel;
    [SerializeField] private TextMeshProUGUI _hapticLabel;

    private AudioManager _audioManager;

    protected override void Awake()
    {
        base.Awake();
        _musicToggle.onClick.AddListener(OnMusicToggle);
        _sfxToggle.onClick.AddListener(OnSFXToggle);
        _hapticToggle.onClick.AddListener(OnHapticToggle);
        _closeButton.onClick.AddListener(OnClose);
    }

    public void SetAudioManager(AudioManager audioManager)
    {
        _audioManager = audioManager;
        RefreshLabels();
    }

    protected override void OnShow()
    {
        base.OnShow();
        if (_audioManager == null)
            _audioManager = AudioManager.Instance;
        RefreshLabels();
    }

    private void OnMusicToggle()
    {
        _audioManager?.ToggleMusic();
        RefreshLabels();
    }

    private void OnSFXToggle()
    {
        _audioManager?.ToggleSFX();
        RefreshLabels();
    }

    private void OnHapticToggle()
    {
        HapticManager.Enabled = !HapticManager.Enabled;
        RefreshLabels();
    }

    private void OnClose()
    {
        Hide();
    }

    private void RefreshLabels()
    {
        if (_audioManager != null)
        {
            _musicLabel.text = $"Music: {(_audioManager.MusicEnabled ? "ON" : "OFF")}";
            _sfxLabel.text = $"SFX: {(_audioManager.SFXEnabled ? "ON" : "OFF")}";
        }
        _hapticLabel.text = $"Haptic: {(HapticManager.Enabled ? "ON" : "OFF")}";
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _musicToggle.onClick.RemoveListener(OnMusicToggle);
        _sfxToggle.onClick.RemoveListener(OnSFXToggle);
        _hapticToggle.onClick.RemoveListener(OnHapticToggle);
        _closeButton.onClick.RemoveListener(OnClose);
    }
}
