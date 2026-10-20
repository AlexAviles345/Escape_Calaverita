using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SkinCatalog",
    menuName = "Escape Calaverita/Skin Catalog")]
public class SkinCatalog : ScriptableObject
{
    public const string SelectedSkinKey = "SelectedSkin";

    [Serializable]
    public class Skin
    {
        [SerializeField] private string displayName = "CALAVERITA";
        [SerializeField] private Sprite sprite;

        public string DisplayName => displayName;
        public Sprite Sprite => sprite;
    }

    [SerializeField] private Skin[] skins;

    public int Count => skins?.Length ?? 0;

    public Skin GetSkin(int index)
    {
        if (Count == 0)
            return null;

        return skins[Mathf.Clamp(index, 0, Count - 1)];
    }

    public int ClampIndex(int index)
    {
        return Count == 0 ? 0 : Mathf.Clamp(index, 0, Count - 1);
    }
}
