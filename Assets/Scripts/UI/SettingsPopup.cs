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

    [SerializeField] private Sprite _toggleOnSprite;
    [SerializeField] private Sprite _toggleOffSprite;

    private AudioManager _audioManager;
    private Image _musicImage;
    private Image _sfxImage;
    private Image _hapticImage;

    protected override void Awake()
    {
        base.Awake();
        _musicImage = _musicToggle.GetComponent<Image>();
        _sfxImage = _sfxToggle.GetComponent<Image>();
        _hapticImage = _hapticToggle.GetComponent<Image>();
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
            bool musicOn = _audioManager.MusicEnabled;
            _musicLabel.text = musicOn ? "ON" : "OFF";
            SetToggleSprite(_musicImage, musicOn);

            bool sfxOn = _audioManager.SFXEnabled;
            _sfxLabel.text = sfxOn ? "ON" : "OFF";
            SetToggleSprite(_sfxImage, sfxOn);
        }

        bool hapticOn = HapticManager.Enabled;
        _hapticLabel.text = hapticOn ? "ON" : "OFF";
        SetToggleSprite(_hapticImage, hapticOn);
    }

    private void SetToggleSprite(Image image, bool isOn)
    {
        var sprite = isOn ? _toggleOnSprite : _toggleOffSprite;
        if (sprite != null && image != null)
            image.sprite = sprite;
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
