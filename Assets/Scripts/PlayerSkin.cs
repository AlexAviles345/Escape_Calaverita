using UnityEngine;

public class PlayerSkin : MonoBehaviour
{
    [SerializeField] private SkinCatalog catalog;
    [SerializeField] private SpriteRenderer targetRenderer;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<SpriteRenderer>();

        ApplySelectedSkin();
    }

    private void ApplySelectedSkin()
    {
        if (catalog == null || targetRenderer == null || catalog.Count == 0)
            return;

        int savedIndex = PlayerPrefs.GetInt(SkinCatalog.SelectedSkinKey, 0);
        int validIndex = catalog.ClampIndex(savedIndex);
        SkinCatalog.Skin selectedSkin = catalog.GetSkin(validIndex);

        if (selectedSkin?.Sprite != null)
            targetRenderer.sprite = selectedSkin.Sprite;

        if (validIndex != savedIndex)
        {
            PlayerPrefs.SetInt(SkinCatalog.SelectedSkinKey, validIndex);
            PlayerPrefs.Save();
        }
    }
}
