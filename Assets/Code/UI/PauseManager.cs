using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("UI 面板引用")]
    public GameObject pausePanel;    // 关联 PausePanel (暂停主页)
    public GameObject settingsPanel; // 关联 SettingsPanel 预制体

    [Header("音频控制")]
    public AudioMixer mainMixer;       
    public Slider volumeSlider;        
    public Toggle musicToggle;         
    public string volumeParameterName = "MainVolume"; 

    [Header("设置项引用")]
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityValueText;
    public Toggle fullscreenToggle;    

    private bool isPaused = false;

    void Start()
    {
        // 1. 初始化设置数值 (确保进游戏时音量和灵敏度是玩家调好的)
        InitSettings();

        // 2. 初始状态关闭所有面板，确保游戏正常开始
        if(pausePanel != null) pausePanel.SetActive(false);
        if(settingsPanel != null) settingsPanel.SetActive(false);
        
        Time.timeScale = 1f; 
        isPaused = false;
    }

    void Update()
    {
        // 监听 Esc 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && settingsPanel.activeSelf)
            {
                CloseSettingsInPause(); // 如果在设置里，按Esc返回暂停主页
            }
            else
            {
                TogglePause();
            }
        }
    }

    // --- 设置项逻辑 ---

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

    // --- 暂停与转场逻辑 ---

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // 冻结游戏世界
        Cursor.lockState = CursorLockMode.None; // 释放鼠标
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f; // 恢复游戏世界
        // 如果是第一人称游戏，取消下面两行注释:
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    public void OpenSettingsInPause()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsInPause()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // --- 【补全】导航逻辑 ---

    // 回到主菜单
    public void BackToMainMenu()
    {
        // 1. 恢复时间，防止主菜单冻结
        Time.timeScale = 1f;

        // 2. 【核心步骤】告诉主界面脚本：我们现在是“回城”模式
        // 因为 MainMenuManager 是公共类，静态变量可以直接通过 类名.变量名 访问
        MainMenuManager.isReturningFromGame = true;

        // 3. 正常跳转回 SampleScene (索引为 0)
        SceneManager.LoadScene(0); 
    }

    // 退出游戏
    public void QuitGame()
    {
        Debug.Log("正在退出游戏...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 编辑器内停止播放
        #else
            Application.Quit(); // 打包后真正关闭程序
        #endif
    }
}