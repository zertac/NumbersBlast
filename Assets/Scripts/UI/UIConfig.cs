using UnityEngine;

namespace NumbersBlast.UI
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "NumbersBlast/UI Config")]
    public class UIConfig : ScriptableObject
    {
        public PopupEntry[] Popups;

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
