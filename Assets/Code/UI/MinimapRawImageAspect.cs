using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RawImage))]
public class MinimapRawImageAspect : MonoBehaviour
{
    public enum AnchorCorner
    {
        TopLeft,
        TopRight
    }

    public RenderTexture minimapTexture;
    public float width = 180f;
    public bool useTextureAspect = true;

    [Header("Layout")]
    public bool applyCornerAnchor = true;
    public AnchorCorner corner = AnchorCorner.TopLeft;
    public Vector2 margin = new Vector2(16f, 16f);
    public bool resetUvRect = true;

    private RawImage rawImage;
    private RectTransform rectTransform;

    private void Awake()
    {
        Apply();
    }

    private void OnValidate()
    {
        Apply();
    }

    private void LateUpdate()
    {
        if (Application.isPlaying)
        {
            Apply();
        }
    }

    private void Apply()
    {
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
        if (rawImage == null || rectTransform == null) return;

        if (minimapTexture != null)
        {
            rawImage.texture = minimapTexture;
        }

        if (resetUvRect)
        {
            rawImage.uvRect = new Rect(0f, 0f, 1f, 1f);
        }

        Texture texture = rawImage.texture;
        float height = width;
        if (useTextureAspect && texture != null && texture.width > 0)
        {
            height = width * texture.height / texture.width;
        }

        if (applyCornerAnchor)
        {
            ApplyCornerAnchor();
        }

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    private void ApplyCornerAnchor()
    {
        if (corner == AnchorCorner.TopRight)
        {
            rectTransform.anchorMin = new Vector2(1f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(1f, 1f);
            rectTransform.anchoredPosition = new Vector2(-margin.x, -margin.y);
        }
        else
        {
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = new Vector2(margin.x, -margin.y);
        }
    }
}
