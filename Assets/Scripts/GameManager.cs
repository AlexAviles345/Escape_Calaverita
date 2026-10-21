using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameStarted { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }
    public int Score { get; private set; }
    public int BestScore { get; private set; }

    [Header("Interfaz de inicio")]
    [SerializeField] private GameObject startPanel;

    [Header("Interfaz durante la partida")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private TMP_Text scoreText;

    [Header("Interfaz de Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text recordText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Interfaz de pausa")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TMP_Text pausePointsText;
    [SerializeField] private TMP_Text pauseRecordText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseMenuButton;

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
        AudioListener.pause = false;
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

        SetHudVisible(true);
    }

    public void AddScore()
    {
        if (!IsGameStarted || IsGameOver || IsPaused) return;

        Score++;
        UpdateScoreText();

        if (Score <= BestScore) return;

        BestScore = Score;
        PlayerPrefs.SetInt(BestScoreKey, BestScore);
        PlayerPrefs.Save();
    }

    public void GameOver()
    {
        if (IsGameOver || !IsGameStarted || IsPaused) return;

        IsGameOver = true;
        gameOverTime = Time.unscaledTime;
        SetHudVisible(false);
        ShowGameOverCanvas();
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (IsGameOver)
        {
            HandleGameOverInput();
            return;
        }

        if (!IsGameStarted || !WasPausePressed()) return;

        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void HandleGameOverInput()
    {
        if (Time.unscaledTime - gameOverTime < 0.5f) return;

        SetGameOverButtonsInteractable(true);

        bool keyboardRestart = Keyboard.current != null &&
                               (Keyboard.current.spaceKey.wasPressedThisFrame ||
                                Keyboard.current.enterKey.wasPressedThisFrame ||
                                Keyboard.current.numpadEnterKey.wasPressedThisFrame);

        if (keyboardRestart)
            RestartGame();
    }

    private static bool WasPausePressed()
    {
        bool keyboard = Keyboard.current != null &&
                        (Keyboard.current.escapeKey.wasPressedThisFrame ||
                         Keyboard.current.pKey.wasPressedThisFrame);
        bool gamepad = Gamepad.current != null &&
                       Gamepad.current.startButton.wasPressedThisFrame;

        return keyboard || gamepad;
    }

    public static bool WasActionPressed()
    {
        bool keyboard = Keyboard.current != null &&
                        (Keyboard.current.spaceKey.wasPressedThisFrame ||
                         Keyboard.current.upArrowKey.wasPressedThisFrame ||
                         Keyboard.current.wKey.wasPressedThisFrame);
        bool mouse = Mouse.current != null &&
                     Mouse.current.leftButton.wasPressedThisFrame &&
                     !IsPointerOverButton(Mouse.current.position.ReadValue());
        bool touch = Touchscreen.current != null &&
                     Touchscreen.current.primaryTouch.press.wasPressedThisFrame &&
                     !IsPointerOverButton(Touchscreen.current.primaryTouch.position.ReadValue());

        return keyboard || mouse || touch;
    }

    private static bool IsPointerOverButton(Vector2 screenPosition)
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null) return false;

        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = screenPosition
        };
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.GetComponentInParent<Button>() != null)
                return true;
        }

        return false;
    }

    private void ConfigureInterface()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (menuButton != null)
            menuButton.onClick.AddListener(LoadMainMenu);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (pauseMenuButton != null)
            pauseMenuButton.onClick.AddListener(LoadMainMenu);

        SetGameOverButtonsInteractable(false);

        if (startPanel != null)
            startPanel.SetActive(!IsGameStarted);

        UpdateScoreText();
        SetHudVisible(IsGameStarted);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void PauseGame()
    {
        if (!IsGameStarted || IsGameOver || IsPaused) return;

        IsPaused = true;
        SetHudVisible(false);
        UpdatePauseTexts();

        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;

        IsPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        SetHudVisible(true);
        AudioListener.pause = false;
        Time.timeScale = 1f;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = Score.ToString();
    }

    private void UpdatePauseTexts()
    {
        if (pausePointsText != null)
            pausePointsText.text = $"PUNTOS: {Score}";

        if (pauseRecordText != null)
            pauseRecordText.text = $"RECORD: {BestScore}";
    }

    private void SetHudVisible(bool visible)
    {
        if (hudPanel != null)
            hudPanel.SetActive(visible);
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
        AudioListener.pause = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private static void LoadMainMenu()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(MenuSceneName);
    }

    private void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);

        if (menuButton != null)
            menuButton.onClick.RemoveListener(LoadMainMenu);

        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(PauseGame);

        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);

        if (pauseMenuButton != null)
            pauseMenuButton.onClick.RemoveListener(LoadMainMenu);

        if (Instance == this)
            Instance = null;
    }
}
