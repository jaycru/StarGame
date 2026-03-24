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
    public AudioMixer mainMixer;       // 拖入你的 Audio Mixer 文件
    public Slider volumeSlider;        // 拖入音量滑动条
    public Toggle musicToggle;         // 拖入音乐开关 Toggle
    public string volumeParameterName = "MainVolume"; // 必须与 Mixer 中暴露的名字一致

    [Header("设置项引用")]
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityValueText;
    public Toggle fullscreenToggle;    // 拖入全屏开关 Toggle

    [Header("存档与版本")]
    public Button continueButton;
    public TextMeshProUGUI versionText;

    private string savePath;

    void Start()
    {
        savePath = Application.persistentDataPath + "/gamesave.json";
        
        // 1. 初始化设置数值
        InitSettings();

        // 2. 核心判断逻辑
        if (isReturningFromGame)
        {
            // --- 情况 A：从游戏场景返回 ---
            // 瞬间隐藏所有不需要的面板，直接显示主菜单
            SetPanelAlpha(splashCG, 0);
            SetPanelAlpha(loadingCG, 0);
            SetPanelAlpha(titleCG, 0);
            SetPanelAlpha(menuCG, 1); // 直接亮起主菜单面板
            
            // 【重要】重置标记，确保下次彻底重启游戏时还能看到 Logo
            isReturningFromGame = false; 
        }
        else
        {
            // --- 情况 B：正常的冷启动 ---
            InitAllPanels(); // 全部隐藏
            StartCoroutine(FullStartSequence()); // 执行 Logo -> 加载 -> 标题 序列
        }

        // 存档检测
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
        // A. 灵敏度读取
        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        if (sensitivitySlider != null) sensitivitySlider.value = savedSensitivity;
        if (sensitivityValueText != null) sensitivityValueText.text = savedSensitivity.ToString("F1");

        // B. 音量滑动条读取
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        if (volumeSlider != null) volumeSlider.value = savedVolume;

        // C. 音乐开关读取 (1代表开, 0代表关)
        int musicStatus = PlayerPrefs.GetInt("MusicStatus", 1);
        if (musicToggle != null) musicToggle.isOn = (musicStatus == 1);

        // D. 全屏状态读取
        int savedFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
        bool isFull = (savedFullscreen == 1);
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFull;
        Screen.fullScreen = isFull;

        // E. 立即应用音频设置
        ApplyVolume(savedVolume);
    }

    // --- 音频与设置项回调函数 ---

    public void OnVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        ApplyVolume(value);
    }

    public void OnMusicToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt("MusicStatus", isOn ? 1 : 0);
        ApplyVolume(volumeSlider.value); // 改变开关后重新计算分贝
    }

    void ApplyVolume(float sliderValue)
    {
        if (mainMixer == null) return;

        float finaldB;
        // 如果开关关闭，强制设为静音(-80分贝)，并让滑动条变灰
        if (musicToggle != null && !musicToggle.isOn)
        {
            finaldB = -80f;
            if (volumeSlider != null) volumeSlider.interactable = false;
        }
        else
        {
            // 如果开关开启，根据滑动条值计算对数分贝
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

    // --- 流程序列协程 ---

    IEnumerator FullStartSequence()
    {
        // 1. 开场 Logo
        if (splashCG != null)
        {
            yield return StartCoroutine(Fade(splashCG, 0, 1, 1.0f)); // 淡入
            yield return new WaitForSeconds(1.5f);                 // 停留
            yield return StartCoroutine(Fade(splashCG, 1, 0, 1.0f)); // 淡出
        }

        // 2. 加载进度
        yield return StartCoroutine(Fade(loadingCG, 0, 1, 0.5f));
        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * loadingSpeed;
            progressBar.value = progress;
            yield return null;
        }
        yield return StartCoroutine(Fade(loadingCG, 1, 0, 0.5f));

        // 3. 显示标题界面
        yield return StartCoroutine(Fade(titleCG, 0, 1, 0.8f));
    }

    // --- 界面切换逻辑 ---

    public void OnTitleStartButtonClick()
    {
        StartCoroutine(SwitchPanel(titleCG, menuCG));
    }

    IEnumerator SwitchPanel(CanvasGroup from, CanvasGroup to)
    {
        StartCoroutine(Fade(from, 1, 0, 0.4f)); 
        yield return StartCoroutine(Fade(to, 0, 1, 0.4f));
    
        from.gameObject.SetActive(false);
    }

    // 二级面板淡入淡出
    public void OpenSettings() { StartCoroutine(Fade(settingsCG, 0, 1, 0.3f)); }
    public void CloseSettings() { StartCoroutine(Fade(settingsCG, 1, 0, 0.3f)); }

    public void OpenDevTeam() { StartCoroutine(Fade(devTeamCG, 0, 1, 0.3f)); }
    public void CloseDevTeam() { StartCoroutine(Fade(devTeamCG, 1, 0, 0.3f)); }

    public void ShowQuitConfirm() { StartCoroutine(Fade(quitConfirmCG, 0, 1, 0.3f)); }
    public void CancelQuit() { StartCoroutine(Fade(quitConfirmCG, 1, 0, 0.3f)); }

    // --- 游戏逻辑 ---

    public void NewGame()
    {
        if (File.Exists(savePath)) File.Delete(savePath); // 删掉旧存档
        SceneManager.LoadScene("GameScene"); // 请确保在 Build Settings 里添加了场景
    }

    public void ContinueGame()
    {
        if (File.Exists(savePath)) SceneManager.LoadScene("GameScene");
    }

    public void ConfirmQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 在编辑器中停止
        #else
            Application.Quit(); // 在打包后的程序中退出
        #endif
    }

    // --- 通用工具函数 ---

    // 实现平滑淡入淡出的核心工具
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
        // 如果 Alpha 大于 0.1，允许该层接收鼠标点击
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