using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Audio")]
    public AudioMixer mainMixer;
    public Slider volumeSlider;
    public Toggle musicToggle;
    public string volumeParameterName = "MainVolume";

    [Header("Settings")]
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityValueText;
    public Toggle fullscreenToggle;

    private enum ExitAction
    {
        None,
        MainMenu,
        QuitApplication
    }

    private bool isPaused = false;
    private ExitAction pendingExitAction = ExitAction.None;
    private CanvasGroup saveExitCG;
    private TextMeshProUGUI saveExitMessageText;
    private TextMeshProUGUI saveExitSaveButtonText;
    private TextMeshProUGUI saveExitNoSaveButtonText;

    private void Start()
    {
        InitSettings();

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (InteractionManager.IsLootListOpen || InteractionManager.WasLootListClosedThisFrame) return;

            if (isPaused && settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettingsInPause();
            }
            else if (saveExitCG != null && saveExitCG.gameObject.activeSelf)
            {
                CancelSaveExit();
            }
            else
            {
                TogglePause();
            }
        }
    }

    private void InitSettings()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        if (sensitivitySlider != null) sensitivitySlider.value = savedSensitivity;
        if (sensitivityValueText != null) sensitivityValueText.text = savedSensitivity.ToString("F1");

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        if (volumeSlider != null) volumeSlider.value = savedVolume;

        int musicStatus = PlayerPrefs.GetInt("MusicStatus", 1);
        if (musicToggle != null) musicToggle.isOn = musicStatus == 1;

        int savedFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
        bool isFull = savedFullscreen == 1;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFull;

        ApplyVolume(savedVolume);
        Screen.fullScreen = isFull;
    }

    public void OnVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        ApplyVolume(value);
    }

    public void OnMusicToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt("MusicStatus", isOn ? 1 : 0);
        if (volumeSlider != null)
        {
            ApplyVolume(volumeSlider.value);
        }
    }

    private void ApplyVolume(float sliderValue)
    {
        if (mainMixer == null) return;

        float finalDb;
        if (musicToggle != null && !musicToggle.isOn)
        {
            finalDb = -80f;
            if (volumeSlider != null) volumeSlider.interactable = false;
        }
        else
        {
            finalDb = Mathf.Log10(Mathf.Max(0.0001f, sliderValue)) * 20f;
            if (volumeSlider != null) volumeSlider.interactable = true;
        }

        mainMixer.SetFloat(volumeParameterName, finalDb);
    }

    public void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        if (sensitivityValueText != null)
        {
            sensitivityValueText.text = value.ToString("F1");
        }
    }

    public void OnFullscreenToggleChanged(bool isFull)
    {
        Screen.fullScreen = isFull;
        PlayerPrefs.SetInt("Fullscreen", isFull ? 1 : 0);
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (saveExitCG != null) saveExitCG.gameObject.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerMoveTest playerScript = FindObjectOfType<PlayerMoveTest>();
        if (playerScript != null) playerScript.UpdateSensitivity();

        TPSCameraControl cameraScript = FindObjectOfType<TPSCameraControl>();
        if (cameraScript != null) cameraScript.UpdateSensitivity();
    }

    public void OpenSettingsInPause()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettingsInPause()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        ShowSaveExitPrompt(ExitAction.MainMenu);
    }

    public void QuitGame()
    {
        ShowSaveExitPrompt(ExitAction.QuitApplication);
    }

    public void SaveAndExit()
    {
        GameSaveManager saveManager = GameSaveManager.Instance != null ? GameSaveManager.Instance : FindObjectOfType<GameSaveManager>();

        if (saveManager != null)
        {
            saveManager.SaveCurrentGame();
        }
        else
        {
            Debug.LogWarning("Save skipped: GameSaveManager was not found.");
        }

        ExecutePendingExit();
    }

    public void ExitWithoutSave()
    {
        ExecutePendingExit();
    }

    public void CancelSaveExit()
    {
        pendingExitAction = ExitAction.None;

        if (saveExitCG != null)
        {
            saveExitCG.gameObject.SetActive(false);
        }
    }

    private void ShowSaveExitPrompt(ExitAction action)
    {
        pendingExitAction = action;
        EnsureSaveExitPanel();

        if (!isPaused)
        {
            PauseGame();
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        bool isQuitApplication = action == ExitAction.QuitApplication;
        saveExitMessageText.text = isQuitApplication ? "退出游戏前是否保存？" : "返回主菜单前是否保存？";
        saveExitSaveButtonText.text = isQuitApplication ? "保存并退出" : "保存并返回";
        saveExitNoSaveButtonText.text = isQuitApplication ? "不保存退出" : "不保存返回";

        saveExitCG.gameObject.SetActive(true);
        saveExitCG.alpha = 1f;
        saveExitCG.interactable = true;
        saveExitCG.blocksRaycasts = true;
        saveExitCG.transform.SetAsLastSibling();
    }

    private void ExecutePendingExit()
    {
        ExitAction action = pendingExitAction;
        pendingExitAction = ExitAction.None;

        Time.timeScale = 1f;

        if (action == ExitAction.MainMenu)
        {
            MainMenuManager.isReturningFromGame = true;
            SceneManager.LoadScene(0);
            return;
        }

        if (action == ExitAction.QuitApplication)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private void EnsureSaveExitPanel()
    {
        if (saveExitCG != null)
        {
            return;
        }

        Transform parent = pausePanel != null && pausePanel.transform.parent != null ? pausePanel.transform.parent : transform;

        GameObject panelObject = new GameObject("SaveExitPanel", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        SetRect(panelRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(520f, 260f), new Vector2(0.5f, 0.5f));

        Image panelImage = panelObject.GetComponent<Image>();
        panelImage.color = new Color(0.06f, 0.07f, 0.08f, 0.96f);

        saveExitCG = panelObject.GetComponent<CanvasGroup>();

        saveExitMessageText = CreateText("Message", panelObject.transform, "退出前是否保存？", 28f, FontStyles.Bold, TextAlignmentOptions.Center);
        SetRect(saveExitMessageText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -34f), new Vector2(0f, 46f), new Vector2(0.5f, 1f));

        Button saveButton = CreateButton(panelObject.transform, "保存并退出", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -104f), new Vector2(260f, 38f), out saveExitSaveButtonText);
        saveButton.onClick.AddListener(SaveAndExit);

        Button noSaveButton = CreateButton(panelObject.transform, "不保存退出", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -154f), new Vector2(260f, 38f), out saveExitNoSaveButtonText);
        noSaveButton.onClick.AddListener(ExitWithoutSave);

        Button cancelButton = CreateButton(panelObject.transform, "取消", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -204f), new Vector2(260f, 38f), out _);
        cancelButton.onClick.AddListener(CancelSaveExit);

        panelObject.SetActive(false);
    }

    private Button CreateButton(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, out TextMeshProUGUI labelText)
    {
        GameObject buttonObject = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        SetRect(buttonObject.GetComponent<RectTransform>(), anchorMin, anchorMax, anchoredPosition, sizeDelta, new Vector2(0.5f, 0.5f));

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.88f, 0.88f, 0.88f, 1f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        labelText = CreateText("Label", buttonObject.transform, label, 20f, FontStyles.Normal, TextAlignmentOptions.Center);
        labelText.color = Color.black;
        SetRect(labelText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        return button;
    }

    private TextMeshProUGUI CreateText(string objectName, Transform parent, string textValue, float fontSize, FontStyles style, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform));
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = textValue;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = Color.white;

        if (sensitivityValueText != null && sensitivityValueText.font != null)
        {
            text.font = sensitivityValueText.font;
        }

        return text;
    }

    private void SetRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Vector2 pivot)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.pivot = pivot;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }
}
