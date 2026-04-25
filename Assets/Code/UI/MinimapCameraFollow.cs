using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MinimapCameraFollow : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;
    public float height = 40f;
    public Vector3 targetOffset;
    public bool rotateWithPlayer = false;

    [Header("Camera")]
    public RenderTexture minimapTexture;
    public float orthographicSize = 30f;

    private Camera minimapCamera;
    private const string PlayerTag = "Player";

    private void Awake()
    {
        minimapCamera = GetComponent<Camera>();
        ApplyCameraSettings();
    }

    private void Reset()
    {
        minimapCamera = GetComponent<Camera>();
        ApplyCameraSettings();
    }

    private void OnValidate()
    {
        minimapCamera = GetComponent<Camera>();
        ApplyCameraSettings();
    }

    private void LateUpdate()
    {
        TryFindTarget();
        if (target == null) return;

        ApplyCameraSettings();

        Vector3 targetPosition = target.position + targetOffset;
        transform.position = new Vector3(targetPosition.x, targetPosition.y + height, targetPosition.z);

        if (rotateWithPlayer)
        {
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    private void ApplyCameraSettings()
    {
        if (minimapCamera == null) return;

        minimapCamera.orthographic = true;
        minimapCamera.orthographicSize = orthographicSize;

        if (minimapTexture != null)
        {
            minimapCamera.targetTexture = minimapTexture;
            minimapCamera.aspect = (float)minimapTexture.width / minimapTexture.height;
        }

        if (Application.isPlaying && minimapCamera.targetTexture == null)
        {
            minimapCamera.enabled = false;
            Debug.LogWarning("MinimapCamera has no RenderTexture. Assign MinimapRT to avoid rendering over the main view.", this);
        }
    }

    private void TryFindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag(PlayerTag);
        if (player == null) return;

        if (target == null || !target.root.CompareTag(PlayerTag))
        {
            target = player.transform;
        }
    }
}

