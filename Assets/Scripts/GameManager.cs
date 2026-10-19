using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameStarted { get; private set; }
    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }
    public int BestScore { get; private set; }

    [Header("Interfaz de inicio")]
    [SerializeField] private GameObject startPanel;

    [Header("Interfaz de Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text recordText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    private float gameOverTime;
    private const string BestScoreKey = "BestScore";
    private const string MenuSceneName = "Menu";
    private static bool startImmediatelyAfterReload;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        IsGameStarted = startImmediatelyAfterReload;
        startImmediatelyAfterReload = false;
        ConfigureInterface();
    }

    public void StartGame()
    {
        if (IsGameStarted || IsGameOver) return;
        IsGameStarted = true;

        if (startPanel != null)
            startPanel.SetActive(false);
    }

    public void AddScore()
    {
        if (!IsGameStarted || IsGameOver) return;

        Score++;
        if (Score <= BestScore) return;

        BestScore = Score;
        PlayerPrefs.SetInt(BestScoreKey, BestScore);
        PlayerPrefs.Save();
    }

    public void GameOver()
    {
        if (IsGameOver || !IsGameStarted) return;

        IsGameOver = true;
        gameOverTime = Time.unscaledTime;
        ShowGameOverCanvas();
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (!IsGameOver) return;
        if (Time.unscaledTime - gameOverTime < 0.5f) return;

        SetGameOverButtonsInteractable(true);

        bool keyboardRestart = Keyboard.current != null &&
                               (Keyboard.current.spaceKey.wasPressedThisFrame ||
                                Keyboard.current.enterKey.wasPressedThisFrame ||
                                Keyboard.current.numpadEnterKey.wasPressedThisFrame);

        if (keyboardRestart)
            RestartGame();
    }

    public static bool WasActionPressed()
    {
        bool keyboard = Keyboard.current != null &&
                        (Keyboard.current.spaceKey.wasPressedThisFrame ||
                         Keyboard.current.upArrowKey.wasPressedThisFrame ||
                         Keyboard.current.wKey.wasPressedThisFrame);
        bool mouse = Mouse.current != null &&
                     Mouse.current.leftButton.wasPressedThisFrame;
        bool touch = Touchscreen.current != null &&
                     Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

        return keyboard || mouse || touch;
    }

    private void OnGUI()
    {
        if (!IsGameStarted || IsGameOver) return;

        float scale = Mathf.Clamp(Screen.height / 720f, 0.75f, 1.5f);
        GUIStyle scoreStyle = CreateCenteredStyle(Mathf.RoundToInt(52f * scale), FontStyle.Bold);

        GUI.Label(new Rect(0f, 24f * scale, Screen.width, 70f * scale),
            Score.ToString(), scoreStyle);
    }

    private static GUIStyle CreateCenteredStyle(int fontSize, FontStyle fontStyle)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = fontSize,
            fontStyle = fontStyle
        };
        style.normal.textColor = new Color(1f, 0.82f, 0.25f);
        return style;
    }

    private void ConfigureInterface()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (menuButton != null)
            menuButton.onClick.AddListener(LoadMainMenu);

        SetGameOverButtonsInteractable(false);

        if (startPanel != null)
            startPanel.SetActive(!IsGameStarted);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void ShowGameOverCanvas()
    {
        if (pointsText != null)
            pointsText.text = $"PUNTOS: {Score}";

        if (recordText != null)
            recordText.text = $"RECORD: {BestScore}";

        SetGameOverButtonsInteractable(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    private void SetGameOverButtonsInteractable(bool interactable)
    {
        if (restartButton != null)
            restartButton.interactable = interactable;

        if (menuButton != null)
            menuButton.interactable = interactable;
    }

    private static void RestartGame()
    {
        startImmediatelyAfterReload = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private static void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MenuSceneName);
    }
}
