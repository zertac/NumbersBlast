using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private readonly UIConfig _config;
    private readonly Transform _popupContainer;
    private readonly Dictionary<PopupType, BasePopup> _cachedPopups = new();
    private BasePopup _currentPopup;

    public UIManager(UIConfig config, Transform popupContainer)
    {
        _config = config;
        _popupContainer = popupContainer;
    }

    public void ShowPopup(PopupType type)
    {
        if (_currentPopup != null)
        {
            _currentPopup.Hide();
            _currentPopup = null;
        }

        if (!_cachedPopups.TryGetValue(type, out var popup))
        {
            var prefab = _config.GetPopupPrefab(type);
            if (prefab == null)
            {
                Debug.LogWarning($"[UIManager] No prefab for popup type: {type}");
                return;
            }

            var go = Object.Instantiate(prefab, _popupContainer);
            popup = go.GetComponent<BasePopup>();
            _cachedPopups[type] = popup;
        }

        popup.Show();
        _currentPopup = popup;
    }

    public void CloseCurrentPopup()
    {
        if (_currentPopup == null) return;
        _currentPopup.Hide();
        _currentPopup = null;
    }

    public T GetPopup<T>(PopupType type) where T : BasePopup
    {
        if (_cachedPopups.TryGetValue(type, out var popup))
            return popup as T;
        return null;
    }

    public bool IsPopupActive => _currentPopup != null && _currentPopup.gameObject.activeSelf;
}
