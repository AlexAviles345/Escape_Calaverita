using UnityEngine;

public class PlayerSkin : MonoBehaviour
{
    [SerializeField] private SkinCatalog catalog;
    [SerializeField] private SpriteRenderer targetRenderer;

    private Vector3 originalVisualScale;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<SpriteRenderer>();

        if (targetRenderer != null)
            originalVisualScale = targetRenderer.transform.localScale;

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
        {
            targetRenderer.sprite = selectedSkin.Sprite;

            float skinScale = catalog.GetScaleRelativeToFirstSkin(validIndex);
            targetRenderer.transform.localScale = new Vector3(
                originalVisualScale.x * skinScale,
                originalVisualScale.y * skinScale,
                originalVisualScale.z);
        }

        if (validIndex != savedIndex)
        {
            PlayerPrefs.SetInt(SkinCatalog.SelectedSkinKey, validIndex);
            PlayerPrefs.Save();
        }
    }
}
