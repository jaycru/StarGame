using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HUDManager : MonoBehaviour
{
    // 单例模式：让其他脚本（如玩家或敌人）能通过 HUDManager.Instance 访问
    public static HUDManager Instance;

    [Header("血条核心组件")]
    public Slider healthSlider;       // 主血条
    public Slider ghostSlider;        // 残影血条（缓缓追赶那个）
    public Image healthFillImage;     // 血条的填充图片（用于改颜色）
    public RectTransform healthGroup; // 整个左上角血条组的容器（用于受击抖动）

    [Header("血条动态设置")]
    public float ghostFollowSpeed = 2f;    // 残影追赶速度
    public Color fullHealthColor = Color.green; // 满血颜色
    public Color lowHealthColor = Color.red;    // 残血颜色

    [Header("子弹 UI 组件")]
    public TextMeshProUGUI ammoText;
    public RectTransform ammoGroup;        // 子弹文字的容器（用于弹跳缩放）

    [Header("交互提示组件")]
    public GameObject hintPanel;
    public TextMeshProUGUI hintText;

    [Header("全屏受击反馈")]
    public CanvasGroup damageCanvasGroup;  // 红色遮罩的 CanvasGroup
    public float flashSpeed = 4f;          // 红色消失速度
    public float maxFlashAlpha = 0.6f;     // 红色最亮透明度

    // 内部状态变量
    private Coroutine damageRoutine;
    private bool isDead = false;           // 死亡锁，防止重复触发死亡界面

    void Awake()
    {
        // 初始化单例
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 1. 初始化数值：确保开始时是满的，防止误判死亡
        healthSlider.value = 1f; 
        if (ghostSlider != null) ghostSlider.value = 1f;
        
        // 2. 颜色初始化
        if (healthFillImage != null) healthFillImage.color = fullHealthColor;

        // 3. 状态初始化
        isDead = false;

        // 4. 其他 UI 隐藏
        if (damageCanvasGroup != null) damageCanvasGroup.alpha = 0;
        if (hintPanel != null) hintPanel.SetActive(false);
    }

    void Update()
    {
        // --- 逻辑美化：残影血条平滑追赶主血条 ---
        if (ghostSlider != null && ghostSlider.value > healthSlider.value)
        {
            ghostSlider.value = Mathf.Lerp(ghostSlider.value, healthSlider.value, Time.deltaTime * ghostFollowSpeed);
        }
    }

    // --- 核心接口 1：更新血条数值与状态 ---
    public void UpdateHealth(float current, float max)
    {
        if (isDead) return; // 如果已经判定死亡，不再更新 UI

        float ratio = current / max;
        healthSlider.value = ratio;

        // 逻辑美化：根据血量比例在绿色和红色之间平滑过渡
        if (healthFillImage != null)
        {
            healthFillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, ratio);
        }

        // --- 核心逻辑：检测是否死亡 ---
        if (current <= 0)
        {
            TriggerDeath();
        }
    }

    // --- 核心接口 2：更新子弹数值 ---
    public void UpdateAmmo(int current, int total)
    {
        if (isDead) return;

        ammoText.text = $"{current} / {total}";
        
        // 逻辑美化：每次子弹变化，UI 弹跳一下
        if (ammoGroup != null)
        {
            StopCoroutine("PunchScale"); // 停止旧动画防止冲突
            StartCoroutine(PunchScale(ammoGroup, 1.2f));
        }
    }

    // --- 核心接口 3：播放受击特效 (全屏红+血条抖) ---
    public void PlayDamageEffect()
    {
        if (isDead) return;

        // 1. 全屏红色闪烁
        if (damageCanvasGroup != null)
        {
            if (damageRoutine != null) StopCoroutine(damageRoutine);
            damageRoutine = StartCoroutine(DamageFlashRoutine());
        }

        // 2. 血条 UI 剧烈抖动
        if (healthGroup != null)
        {
            StartCoroutine(ShakeUI(healthGroup, 10f, 0.2f));
        }
    }

    // --- 核心接口 4：显示交互提示 ---
    public void SetInteractionHint(bool show, string message = "")
    {
        if (hintPanel == null) return;
        hintText.text = message;
        hintPanel.SetActive(show);
    }

    // --- 内部死亡处理逻辑 ---
    private void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;
        
        Debug.Log("【逻辑检查】玩家已经死了，准备呼叫死亡界面..."); // 加这行

        PlayDamageEffect();

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.SetupGameOver();
        }
        else
        {
            Debug.LogError("【严重错误】场景中找不到 GameOverManager 的实例！");
        }
    }

    // --- 通用动效工具函数 (Juice Tools) ---

    // 缩放弹跳协程
    IEnumerator PunchScale(RectTransform target, float multiplier)
    {
        target.localScale = Vector3.one * multiplier;
        float elapsed = 0;
        while (elapsed < 0.2f)
        {
            elapsed += Time.unscaledDeltaTime;
            target.localScale = Vector3.Lerp(target.localScale, Vector3.one, elapsed / 0.2f);
            yield return null;
        }
        target.localScale = Vector3.one;
    }

    // UI 抖动协程
    IEnumerator ShakeUI(RectTransform target, float intensity, float duration)
    {
        Vector2 originalPos = target.anchoredPosition;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);
            target.anchoredPosition = originalPos + new Vector2(x, y);
            yield return null;
        }
        target.anchoredPosition = originalPos;
    }

    // 受击闪红协程
    IEnumerator DamageFlashRoutine()
    {
        damageCanvasGroup.alpha = maxFlashAlpha;
        while (damageCanvasGroup.alpha > 0)
        {
            damageCanvasGroup.alpha -= Time.unscaledDeltaTime * flashSpeed;
            yield return null;
        }
        damageCanvasGroup.alpha = 0;
    }
}