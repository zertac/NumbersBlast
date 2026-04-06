using UnityEngine;

namespace NumbersBlast.UI
{
    /// <summary>
    /// ScriptableObject configuration that holds registered popup prefabs for the UI system.
    /// </summary>
    [CreateAssetMenu(fileName = "UIConfig", menuName = "NumbersBlast/UI Config")]
    public class UIConfig : ScriptableObject
    {
        public PopupEntry[] Popups;

        /// <summary>
        /// Returns the prefab GameObject for the specified popup type, or null if not found.
        /// </summary>
        public GameObject GetPopupPrefab<T>() where T : BasePopup
        {
            var targetType = typeof(T);
            for (int i = 0; i < Popups.Length; i++)
            {
                if (Popups[i].Prefab != null && Popups[i].Prefab.GetComponent<T>() != null)
                    return Popups[i].Prefab;
            }
            return null;
        }
    }

}
