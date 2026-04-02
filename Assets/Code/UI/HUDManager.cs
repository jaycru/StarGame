using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("血条核心 (Health Bar)")]
    public Slider healthSlider;       // 主血条（扣血瞬间变短）
    public Slider ghostSlider;        // 残影血条（缓缓追赶）
    public Image healthFillImage;     // 填充图片（用于改颜色）
    public RectTransform healthGroup; // 整个血条组（用于受击抖动）

    [Header("血条动态设置")]
    public float ghostFollowSpeed = 2f; 
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    [Header("子弹 UI (Ammo)")]
    public TextMeshProUGUI ammoText;
    public RectTransform ammoGroup;   // 用于子弹弹跳缩放

    [Header("交互提示 (Hint)")]
    public GameObject hintPanel;
    public TextMeshProUGUI hintText;

    [Header("受击反馈 (Flash)")]
    public CanvasGroup damageCanvasGroup;
    public float flashSpeed = 4f;

    private Coroutine damageRoutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 初始同步残影条
        if (ghostSlider != null) ghostSlider.value = healthSlider.value;
        if (damageCanvasGroup != null) damageCanvasGroup.alpha = 0;
        if (hintPanel != null) hintPanel.SetActive(false);
    }

    void Update()
    {
        // --- 逻辑美化 1：残影条平滑追赶 ---
        if (ghostSlider != null && ghostSlider.value > healthSlider.value)
        {
            ghostSlider.value = Mathf.Lerp(ghostSlider.value, healthSlider.value, Time.deltaTime * ghostFollowSpeed);
        }
    }

    // --- 核心接口 1：更新血条 ---
    public void UpdateHealth(float current, float max)
    {
        float ratio = current / max;
        healthSlider.value = ratio;

        // 逻辑美化 2：根据血量百分比平滑变色
        healthFillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, ratio);
    }

    // --- 核心接口 2：更新子弹 ---
    public void UpdateAmmo(int current, int total)
    {
        ammoText.text = $"{current} / {total}";
        
        // 逻辑美化 3：子弹改变时弹跳一下
        StartCoroutine(PunchScale(ammoGroup, 1.2f));
    }

    // --- 核心接口 3：播放受击特效 ---
    public void PlayDamageEffect()
    {
        // 逻辑美化 4：全屏闪红
        if (damageRoutine != null) StopCoroutine(damageRoutine);
        damageRoutine = StartCoroutine(DamageFlashRoutine());

        // 逻辑美化 5：血条 UI 剧烈抖动
        StartCoroutine(ShakeUI(healthGroup, 10f, 0.2f));
    }

    public void SetInteractionHint(bool show, string message = "")
    {
        hintText.text = message;
        hintPanel.SetActive(show);
    }

    // --- 通用动效工具 (Juice Tools) ---

    // 缩放弹跳：让 UI 凸显变化
    IEnumerator PunchScale(RectTransform target, float multiplier)
    {
        target.localScale = Vector3.one * multiplier;
        float elapsed = 0;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(target.localScale, Vector3.one, elapsed / 0.2f);
            yield return null;
        }
        target.localScale = Vector3.one;
    }

    // UI 抖动：增加受击力度感
    IEnumerator ShakeUI(RectTransform target, float intensity, float duration)
    {
        Vector2 originalPos = target.anchoredPosition;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);
            target.anchoredPosition = originalPos + new Vector2(x, y);
            yield return null;
        }
        target.anchoredPosition = originalPos;
    }

    IEnumerator DamageFlashRoutine()
    {
        damageCanvasGroup.alpha = 0.6f;
        while (damageCanvasGroup.alpha > 0)
        {
            damageCanvasGroup.alpha -= Time.deltaTime * flashSpeed;
            yield return null;
        }
        damageCanvasGroup.alpha = 0;
    }
}