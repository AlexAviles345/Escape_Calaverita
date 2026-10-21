using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    [Header("Paneles")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject skinPanel;

    [Header("Navegacion")]
    [SerializeField] private Button skinsButton;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        if (skinsButton != null)
            skinsButton.onClick.AddListener(OpenSkinPanel);

        if (backButton != null)
            backButton.onClick.AddListener(CloseSkinPanel);

        CloseSkinPanel();
    }

    private void OnDestroy()
    {
        if (skinsButton != null)
            skinsButton.onClick.RemoveListener(OpenSkinPanel);

        if (backButton != null)
            backButton.onClick.RemoveListener(CloseSkinPanel);
    }

    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSkinPanel()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (skinPanel != null)
            skinPanel.SetActive(true);
    }

    public void CloseSkinPanel()
    {
        if (skinPanel != null)
            skinPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }
}
