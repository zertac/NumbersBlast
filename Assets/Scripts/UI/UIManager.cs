using System;
using System.Collections.Generic;
using UnityEngine;

namespace NumbersBlast.UI
{
    public class UIManager : IUIManager
    {
        private readonly UIConfig _config;
        private readonly Transform _popupContainer;
        private readonly Dictionary<Type, BasePopup> _cachedPopups = new();
        private BasePopup _currentPopup;

        public UIManager(UIConfig config, Transform popupContainer)
        {
            _config = config;
            _popupContainer = popupContainer;
        }

        public T ShowPopup<T>() where T : BasePopup
        {
            if (_currentPopup != null)
            {
                _currentPopup.Hide();
                _currentPopup = null;
            }

            var type = typeof(T);

            if (!_cachedPopups.TryGetValue(type, out var popup))
            {
                var prefab = _config.GetPopupPrefab<T>();
                if (prefab == null)
                {
                    Debug.LogWarning($"[UIManager] No prefab for popup type: {type.Name}");
                    return null;
                }

                var go = UnityEngine.Object.Instantiate(prefab, _popupContainer);
                popup = go.GetComponent<BasePopup>();
                _cachedPopups[type] = popup;
            }

            popup.Show();
            _currentPopup = popup;
            return popup as T;
        }

        public void CloseCurrentPopup()
        {
            if (_currentPopup == null) return;
            _currentPopup.Hide();
            _currentPopup = null;
        }

        public T GetPopup<T>() where T : BasePopup
        {
            if (_cachedPopups.TryGetValue(typeof(T), out var popup))
                return popup as T;
            return null;
        }

        public bool IsPopupActive => _currentPopup != null && _currentPopup.gameObject.activeSelf;
    }
}
