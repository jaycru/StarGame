using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio; // 音频混合器命名空间
using System.Collections;
using System.IO;
using TMPro; // TextMeshPro 命名空间

public class MainMenuManager : MonoBehaviour
{
    public static bool isReturningFromGame = false; 

    [Header("3D 背景模型")]
    public GameObject menuBackgroundModel; // 【新增】拖入场景中的 FBX 模型物体

    [Header("UI 面板 (需挂载 Canvas Group 组件)")]
    public CanvasGroup splashCG;      // 开场 Logo 面板
    public CanvasGroup loadingCG;     // 加载面板
    public CanvasGroup titleCG;       // 初始标题面板
    public CanvasGroup menuCG;        // 主菜单面板

    [Header("二级弹窗 (需挂载 Canvas Group)")]
    public CanvasGroup settingsCG;    // 设置面板
    public CanvasGroup devTeamCG;     // 开发团队面板
    public CanvasGroup quitConfirmCG; // 退出确认面板

    [Header("加载条设置")]
    public Slider progressBar;
    public float loadingSpeed = 0.5f;

    [Header("音频控制")]
    public AudioMixer mainMixer;       
    public Slider volumeSlider;        
    public Toggle musicToggle;         
    public string volumeParameterName = "MainVolume"; 

    [Header("设置项引用")]
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityValueText;
    public Toggle fullscreenToggle;    

    [Header("存档与版本")]
    public Button continueButton;
    public TextMeshProUGUI versionText;

    private string savePath;

    void Start()
    {
        savePath = Application.persistentDataPath + "/gamesave.json";
        
        // 1. 【核心动作】强制初始化所有面板状态
        // 不管编辑器里是怎么摆的，代码运行的第一秒，全部关掉！
        InitAllPanels(); 
        InitSettings();

        // 2. 判断回城逻辑
        if (isReturningFromGame)
        {
            // --- 情况 A：从游戏场景返回 ---
            // 显式确保只有主菜单面板亮起，其他所有 CG 必须是隐藏且不可交互的
            SetPanelAlpha(splashCG, 0);
            SetPanelAlpha(loadingCG, 0);
            SetPanelAlpha(titleCG, 0);
            
            SetPanelAlpha(menuCG, 1); // 只让主菜单亮起
            
            // 额外的保险：强制关闭所有二级弹窗（即便 InitAllPanels 已经做过）
            SetPanelAlpha(settingsCG, 0);
            SetPanelAlpha(devTeamCG, 0);
            SetPanelAlpha(quitConfirmCG, 0);

            // 如果你有 3D 背景模型，记得在这里开启
            if (menuBackgroundModel != null) menuBackgroundModel.SetActive(true);

            isReturningFromGame = false; 
        }
        else
        {
            // --- 情况 B：正常的冷启动 ---
            StartCoroutine(FullStartSequence()); 
        }

        if (continueButton != null)
            continueButton.interactable = File.Exists(savePath);
    }
    // --- 初始化与设置逻辑 ---

    void InitAllPanels()
    {
        SetPanelAlpha(splashCG, 0);
        SetPanelAlpha(loadingCG, 0);
        SetPanelAlpha(titleCG, 0);
        SetPanelAlpha(menuCG, 0);
        SetPanelAlpha(settingsCG, 0);
        SetPanelAlpha(devTeamCG, 0);
        SetPanelAlpha(quitConfirmCG, 0);
    }

    void InitSettings()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        if (sensitivitySlider != null) sensitivitySlider.value = savedSensitivity;
        if (sensitivityValueText != null) sensitivityValueText.text = savedSensitivity.ToString("F1");

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        if (volumeSlider != null) volumeSlider.value = savedVolume;

        int musicStatus = PlayerPrefs.GetInt("MusicStatus", 1);
        if (musicToggle != null) musicToggle.isOn = (musicStatus == 1);

        int savedFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
        bool isFull = (savedFullscreen == 1);
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
        ApplyVolume(volumeSlider.value); 
    }

    void ApplyVolume(float sliderValue)
    {
        if (mainMixer == null) return;
        float finaldB;
        if (musicToggle != null && !musicToggle.isOn)
        {
            finaldB = -80f;
            if (volumeSlider != null) volumeSlider.interactable = false;
        }
        else
        {
            finaldB = Mathf.Log10(Mathf.Max(0.0001f, sliderValue)) * 20;
            if (volumeSlider != null) volumeSlider.interactable = true;
        }
        mainMixer.SetFloat(volumeParameterName, finaldB);
    }

    public void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        if (sensitivityValueText != null) sensitivityValueText.text = value.ToString("F1");
    }

    public void OnFullscreenToggleChanged(bool isFull)
    {
        Screen.fullScreen = isFull;
        PlayerPrefs.SetInt("Fullscreen", isFull ? 1 : 0);
    }

    IEnumerator FullStartSequence()
    {
        if (splashCG != null)
        {
            yield return StartCoroutine(Fade(splashCG, 0, 1, 1.0f)); 
            yield return new WaitForSeconds(1.5f);                 
            yield return StartCoroutine(Fade(splashCG, 1, 0, 1.0f)); 
        }

        yield return StartCoroutine(Fade(loadingCG, 0, 1, 0.5f));
        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * loadingSpeed;
            progressBar.value = progress;
            yield return null;
        }
        yield return StartCoroutine(Fade(loadingCG, 1, 0, 0.5f));
        yield return StartCoroutine(Fade(titleCG, 0, 1, 0.8f));
    }

    public void OnTitleStartButtonClick()
    {
        StartCoroutine(SwitchPanel(titleCG, menuCG));
    }

    IEnumerator SwitchPanel(CanvasGroup from, CanvasGroup to)
    {
        // 【新增逻辑】如果是切换到菜单面板，则开启模型
        if (to == menuCG && menuBackgroundModel != null)
        {
            menuBackgroundModel.SetActive(true);
        }

        StartCoroutine(Fade(from, 1, 0, 0.4f)); 
        yield return StartCoroutine(Fade(to, 0, 1, 0.4f));
    
        from.gameObject.SetActive(false);
    }

    public void OpenSettings() { StartCoroutine(Fade(settingsCG, 0, 1, 0.3f)); }
    public void CloseSettings() { StartCoroutine(Fade(settingsCG, 1, 0, 0.3f)); }

    public void OpenDevTeam() { StartCoroutine(Fade(devTeamCG, 0, 1, 0.3f)); }
    public void CloseDevTeam() { StartCoroutine(Fade(devTeamCG, 1, 0, 0.3f)); }

    public void ShowQuitConfirm() { StartCoroutine(Fade(quitConfirmCG, 0, 1, 0.3f)); }
    public void CancelQuit() { StartCoroutine(Fade(quitConfirmCG, 1, 0, 0.3f)); }

    public void NewGame()
    {
        if (File.Exists(savePath)) File.Delete(savePath); 
        SceneManager.LoadScene("GameScene"); 
    }

    public void ContinueGame()
    {
        if (File.Exists(savePath)) SceneManager.LoadScene("GameScene");
    }

    public void ConfirmQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; 
        #else
            Application.Quit(); 
        #endif
    }

    IEnumerator Fade(CanvasGroup cg, float start, float end, float duration)
    {
        if (cg == null) yield break;
        if (end > 0) cg.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
        if (end <= 0) cg.gameObject.SetActive(false);
        cg.blocksRaycasts = (end > 0.1f);
    }

    void SetPanelAlpha(CanvasGroup cg, float alpha)
    {
        if (cg == null) return;
        cg.alpha = alpha;
        cg.blocksRaycasts = (alpha > 0.1f);
        cg.gameObject.SetActive(alpha > 0);
    }
}