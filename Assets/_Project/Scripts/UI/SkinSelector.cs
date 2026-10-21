using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelector : MonoBehaviour
{
    [SerializeField] private SkinCatalog catalog;
    [SerializeField] private Image previewImage;
    [SerializeField] private TMP_Text skinNameText;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    private int selectedIndex;

    private void Awake()
    {
        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousSkin);

        if (nextButton != null)
            nextButton.onClick.AddListener(NextSkin);

        int savedIndex = PlayerPrefs.GetInt(SkinCatalog.SelectedSkinKey, 0);
        selectedIndex = catalog != null ? catalog.ClampIndex(savedIndex) : 0;
        RefreshSelection(false);
    }

    private void OnDestroy()
    {
        if (previousButton != null)
            previousButton.onClick.RemoveListener(PreviousSkin);

        if (nextButton != null)
            nextButton.onClick.RemoveListener(NextSkin);
    }

    public void PreviousSkin()
    {
        if (catalog == null || catalog.Count == 0)
            return;

        selectedIndex = (selectedIndex - 1 + catalog.Count) % catalog.Count;
        RefreshSelection(true);
    }

    public void NextSkin()
    {
        if (catalog == null || catalog.Count == 0)
            return;

        selectedIndex = (selectedIndex + 1) % catalog.Count;
        RefreshSelection(true);
    }

    private void RefreshSelection(bool saveSelection)
    {
        SkinCatalog.Skin skin = catalog?.GetSkin(selectedIndex);
        bool hasSkin = skin != null;

        if (previewImage != null)
        {
            previewImage.preserveAspect = true;
            previewImage.sprite = hasSkin ? skin.Sprite : null;
            previewImage.enabled = hasSkin && skin.Sprite != null;
        }

        if (skinNameText != null)
            skinNameText.text = hasSkin ? skin.DisplayName : "SIN SKINS";

        bool canBrowse = catalog != null && catalog.Count > 1;
        if (previousButton != null)
            previousButton.interactable = canBrowse;
        if (nextButton != null)
            nextButton.interactable = canBrowse;

        if (!hasSkin || !saveSelection)
            return;

        PlayerPrefs.SetInt(SkinCatalog.SelectedSkinKey, selectedIndex);
        PlayerPrefs.Save();
    }
}
