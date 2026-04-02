using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("死亡界面组件")]
    public CanvasGroup gameOverCG;

    void Start()
    {
        // 强制初始化：确保游戏开始时死亡界面是完全透明且不挡鼠标的
        if (gameOverCG != null)
        {
            gameOverCG.alpha = 0;
            gameOverCG.interactable = false;
            gameOverCG.blocksRaycasts = false;
            gameOverCG.gameObject.SetActive(false); // 彻底关掉物体
        }
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // 核心函数：由 HUDManager 调用
    public void SetupGameOver()
    {
        Debug.Log("GameOverManager: 收到呼叫，准备激活面板...");
    
        // 关键修复：在启动协程前，必须先激活物体，否则协程不会运行
        if (gameOverCG != null)
        {
            gameOverCG.gameObject.SetActive(true); 
            StartCoroutine(FadeInDeathScreen());
        }
        else
        {
            Debug.LogError("GameOverManager: gameOverCG 变量未分配！");
        }
    }

    IEnumerator FadeInDeathScreen()
    {
        Debug.Log("GameOverManager: 协程已经开始运行！");
        
        // 释放鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        float elapsed = 0;
        while (elapsed < 2f)
        {
            elapsed += Time.unscaledDeltaTime; 
            gameOverCG.alpha = elapsed / 2f;
            
            // 打印当前透明度，看看数值有没有动
            // Debug.Log("当前死亡界面透明度: " + gameOverCG.alpha); 
            
            yield return null;
        }

        gameOverCG.alpha = 1;
        gameOverCG.interactable = true;
        gameOverCG.blocksRaycasts = true;
        
        Debug.Log("GameOverManager: 死亡界面已完全显示");
    }
    // --- 按钮绑定的函数 ---

    public void RestartGame()
    {
        Time.timeScale = 1f; // 务必恢复时间
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 重新加载当前关卡
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        MainMenuManager.isReturningFromGame = true; // 复用你之前的逻辑跳过Logo
        SceneManager.LoadScene(0);
    }
}