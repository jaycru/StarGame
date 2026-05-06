using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static bool isReturningFromGame = false;

    [Header("3D Background")]
    public GameObject menuBackgroundModel;

    [Header("Main Panels")]
    public CanvasGroup splashCG;
    public CanvasGroup loadingCG;
    public CanvasGroup titleCG;
    public CanvasGroup menuCG;

    [Header("Popup Panels")]
    public CanvasGroup settingsCG;
    public CanvasGroup devTeamCG;
    public CanvasGroup quitConfirmCG;

    [Header("Loading")]
    public Slider progressBar;
    public float loadingSpeed = 0.5f;

    [Header("Audio")]
    public AudioMixer mainMixer;
    public Slider volumeSlider;
    public Toggle musicToggle;
    public string volumeParameterName = "MainVolume";

    [Header("Settings")]
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityValueText;
    public Toggle fullscreenToggle;

    [Header("Save And Version")]
    public Button continueButton;
    public TextMeshProUGUI versionText;

    private CanvasGroup saveListCG;
    private Transform saveListContent;
    private TextMeshProUGUI saveListEmptyText;

    private void Start()
    {
        InitAllPanels();
        InitSettings();

        if (isReturningFromGame)
        {
            SetPanelAlpha(splashCG, 0f);
            SetPanelAlpha(loadingCG, 0f);
            SetPanelAlpha(titleCG, 0f);
            SetPanelAlpha(menuCG, 1f);
            SetPanelAlpha(settingsCG, 0f);
            SetPanelAlpha(devTeamCG, 0f);
            SetPanelAlpha(quitConfirmCG, 0f);

            if (menuBackgroundModel != null)
            {
                menuBackgroundModel.SetActive(true);
            }

            isReturningFromGame = false;
        }
        else
        {
            StartCoroutine(FullStartSequence());
        }

        RefreshContinueButton();
    }

    private void InitAllPanels()
    {
        SetPanelAlpha(splashCG, 0f);
        SetPanelAlpha(loadingCG, 0f);
        SetPanelAlpha(titleCG, 0f);
        SetPanelAlpha(menuCG, 0f);
        SetPanelAlpha(settingsCG, 0f);
        SetPanelAlpha(devTeamCG, 0f);
        SetPanelAlpha(quitConfirmCG, 0f);
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
        Screen.fullScreen = isFull;

        ApplyVolume(savedVolume);
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

    private IEnumerator FullStartSequence()
    {
        if (splashCG != null)
        {
            yield return StartCoroutine(Fade(splashCG, 0f, 1f, 1.0f));
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(Fade(splashCG, 1f, 0f, 1.0f));
        }

        yield return StartCoroutine(Fade(loadingCG, 0f, 1f, 0.5f));

        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * loadingSpeed;
            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            yield return null;
        }

        yield return StartCoroutine(Fade(loadingCG, 1f, 0f, 0.5f));
        yield return StartCoroutine(Fade(titleCG, 0f, 1f, 0.8f));
    }

    public void OnTitleStartButtonClick()
    {
        StartCoroutine(SwitchPanel(titleCG, menuCG));
    }

    private IEnumerator SwitchPanel(CanvasGroup from, CanvasGroup to)
    {
        if (to == menuCG && menuBackgroundModel != null)
        {
            menuBackgroundModel.SetActive(true);
        }

        StartCoroutine(Fade(from, 1f, 0f, 0.4f));
        yield return StartCoroutine(Fade(to, 0f, 1f, 0.4f));

        if (from != null)
        {
            from.gameObject.SetActive(false);
        }

        RefreshContinueButton();
    }

    public void OpenSettings() { StartCoroutine(Fade(settingsCG, 0f, 1f, 0.3f)); }
    public void CloseSettings() { StartCoroutine(Fade(settingsCG, 1f, 0f, 0.3f)); }
    public void OpenDevTeam() { StartCoroutine(Fade(devTeamCG, 0f, 1f, 0.3f)); }
    public void CloseDevTeam() { StartCoroutine(Fade(devTeamCG, 1f, 0f, 0.3f)); }
    public void ShowQuitConfirm() { StartCoroutine(Fade(quitConfirmCG, 0f, 1f, 0.3f)); }
    public void CancelQuit() { StartCoroutine(Fade(quitConfirmCG, 1f, 0f, 0.3f)); }

    public void NewGame()
    {
        LoadRequest.Clear();
        SceneManager.LoadScene("GameScene");
    }

    public void ContinueGame()
    {
        OpenSaveList();
    }

    public void CloseSaveList()
    {
        SetPanelAlpha(saveListCG, 0f);
    }

    private void OpenSaveList()
    {
        EnsureSaveListPanel();

        foreach (Transform child in saveListContent)
        {
            Destroy(child.gameObject);
        }

        List<SaveData> saves = SaveSystem.GetAllSaves();
        saveListEmptyText.gameObject.SetActive(saves.Count == 0);

        foreach (SaveData save in saves)
        {
            string label = save.saveName + "\n" + save.saveTime + "  |  " + save.sceneName;
            Button button = CreateSaveButton(saveListContent, label);
            SaveData capturedSave = save;
            button.onClick.AddListener(() => LoadSelectedSave(capturedSave));
        }

        SetPanelAlpha(saveListCG, 1f);
        saveListCG.transform.SetAsLastSibling();
    }

    private void LoadSelectedSave(SaveData save)
    {
        if (save == null)
        {
            return;
        }

        LoadRequest.RequestLoad(save.saveId);
        SceneManager.LoadScene(save.sceneName);
    }

    private void RefreshContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.interactable = SaveSystem.HasAnySave();
        }
    }

    public void ConfirmQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void EnsureSaveListPanel()
    {
        if (saveListCG != null)
        {
            return;
        }

        Transform parent = menuCG != null && menuCG.transform.parent != null ? menuCG.transform.parent : transform;

        GameObject panelObject = new GameObject("SaveListPanel", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        SetRect(panelRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(620f, 460f), new Vector2(0.5f, 0.5f));

        Image panelImage = panelObject.GetComponent<Image>();
        panelImage.color = new Color(0.06f, 0.07f, 0.08f, 0.96f);

        saveListCG = panelObject.GetComponent<CanvasGroup>();

        TextMeshProUGUI title = CreateText("Title", panelObject.transform, "选择存档", 30f, FontStyles.Bold, TextAlignmentOptions.Center);
        SetRect(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -22f), new Vector2(0f, 44f), new Vector2(0.5f, 1f));

        saveListEmptyText = CreateText("EmptyText", panelObject.transform, "没有可用存档", 24f, FontStyles.Normal, TextAlignmentOptions.Center);
        SetRect(saveListEmptyText.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), Vector2.zero, new Vector2(0f, 40f), new Vector2(0.5f, 0.5f));

        GameObject scrollObject = new GameObject("SaveScroll", typeof(RectTransform), typeof(ScrollRect));
        scrollObject.transform.SetParent(panelObject.transform, false);
        RectTransform scrollRectTransform = scrollObject.GetComponent<RectTransform>();
        SetRect(scrollRectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, -24f), new Vector2(-56f, -132f), new Vector2(0.5f, 0.5f));

        GameObject viewportObject = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewportObject.transform.SetParent(scrollObject.transform, false);
        RectTransform viewportRect = viewportObject.GetComponent<RectTransform>();
        SetRect(viewportRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));
        viewportObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.03f);
        viewportObject.GetComponent<Mask>().showMaskGraphic = false;

        GameObject contentObject = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        contentObject.transform.SetParent(viewportObject.transform, false);
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;

        VerticalLayoutGroup layout = contentObject.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 8f;
        layout.padding = new RectOffset(8, 8, 8, 8);
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        ContentSizeFitter fitter = contentObject.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scrollRect = scrollObject.GetComponent<ScrollRect>();
        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;
        scrollRect.horizontal = false;

        saveListContent = contentObject.transform;

        Button closeButton = CreateFixedButton(panelObject.transform, "关闭", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 24f), new Vector2(140f, 38f));
        closeButton.onClick.AddListener(CloseSaveList);

        SetPanelAlpha(saveListCG, 0f);
    }

    private Button CreateSaveButton(Transform parent, string label)
    {
        GameObject buttonObject = new GameObject("SaveButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.88f, 0.88f, 0.88f, 1f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = 68f;

        TextMeshProUGUI text = CreateText("Label", buttonObject.transform, label, 20f, FontStyles.Normal, TextAlignmentOptions.Left);
        text.color = Color.black;
        SetRect(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(16f, 0f), new Vector2(-32f, 0f), new Vector2(0.5f, 0.5f));

        return button;
    }

    private Button CreateFixedButton(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject buttonObject = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        SetRect(buttonObject.GetComponent<RectTransform>(), anchorMin, anchorMax, anchoredPosition, sizeDelta, new Vector2(0.5f, 0.5f));

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.88f, 0.88f, 0.88f, 1f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        TextMeshProUGUI text = CreateText("Label", buttonObject.transform, label, 20f, FontStyles.Normal, TextAlignmentOptions.Center);
        text.color = Color.black;
        SetRect(text.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

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

        if (versionText != null && versionText.font != null)
        {
            text.font = versionText.font;
        }

        return text;
    }

    private IEnumerator Fade(CanvasGroup cg, float start, float end, float duration)
    {
        if (cg == null) yield break;

        if (end > 0f)
        {
            cg.gameObject.SetActive(true);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        cg.alpha = end;
        cg.interactable = end > 0.1f;
        cg.blocksRaycasts = end > 0.1f;

        if (end <= 0f)
        {
            cg.gameObject.SetActive(false);
        }
    }

    private void SetPanelAlpha(CanvasGroup cg, float alpha)
    {
        if (cg == null) return;

        cg.alpha = alpha;
        cg.interactable = alpha > 0.1f;
        cg.blocksRaycasts = alpha > 0.1f;
        cg.gameObject.SetActive(alpha > 0f);
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
