using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "NumbersBlast/UI Config")]
public class UIConfig : ScriptableObject
{
    public PopupEntry[] Popups;

    public GameObject GetPopupPrefab(PopupType type)
    {
        for (int i = 0; i < Popups.Length; i++)
        {
            if (Popups[i].Type == type)
                return Popups[i].Prefab;
        }
        return null;
    }
}

[Serializable]
public struct PopupEntry
{
    public PopupType Type;
    public GameObject Prefab;
}
