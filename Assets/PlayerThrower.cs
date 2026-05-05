using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerThrower : MonoBehaviour
{
    [Header("投掷物设置")]
    public GameObject grenade;
    public GameObject smoke;
    public GameObject fire;

    [Header("投掷力度")]
    public float throwForce = 15f;

    [Header("手持位置偏移")]
    public Vector3 holdOffset = new Vector3(0.5f, -0.2f, 1.0f);

    [Header("轨迹设置")]
    public int segments = 20;

    private GameObject currentHeldItem;
    private LineRenderer lineRenderer;
    private bool isHolding = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4) && grenade != null)
        {
            SwitchItem(grenade);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && smoke != null)
        {
            SwitchItem(smoke);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) && fire != null)
        {
            SwitchItem(fire);
        }

        if (currentHeldItem == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Throw();
        }
        else if (Input.GetMouseButton(0))
        {
            UpdateLineRenderer();
        }
        else
        {
            lineRenderer.enabled = false;
        }

        UpdateHeldItemPosition();
    }

    void SwitchItem(GameObject item)
    {
        if (currentHeldItem != null)
        {
            Destroy(currentHeldItem);
        }

        currentHeldItem = Instantiate(item, transform.position + holdOffset, transform.rotation);
        currentHeldItem.GetComponent<Rigidbody>().isKinematic = true;
        currentHeldItem.GetComponent<Collider>().enabled = false;
        isHolding = true;
    }

    void UpdateHeldItemPosition()
    {
        if (currentHeldItem != null)
        {
            currentHeldItem.transform.position = transform.position + holdOffset;
            currentHeldItem.transform.rotation = transform.rotation;
        }
    }

    void Throw()
    {
        if (currentHeldItem == null) return;

        // 修复点：生成位置向前移动 1.5米，避免生成在玩家身体里导致被物理弹飞
        Vector3 spawnPos = transform.position + transform.forward * 1.5f;

        GameObject grenadeInstance = Instantiate(currentHeldItem, spawnPos, transform.rotation);

        Rigidbody rb = grenadeInstance.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = Vector3.zero; // 清除继承的速度
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);

        Destroy(currentHeldItem);
        currentHeldItem = null;
        isHolding = false;
        lineRenderer.enabled = false;
    }

    void UpdateLineRenderer()
    {
        if (currentHeldItem == null) return;

        lineRenderer.enabled = true;
        Vector3 startPos = transform.position + transform.forward * 1.5f; // 轨迹起点也要对应前移
        lineRenderer.positionCount = segments + 1;
        lineRenderer.SetPosition(0, startPos);

        Vector3 velocity = transform.forward * throwForce;

        for (int i = 1; i < segments + 1; i++)
        {
            float t = i / (float)segments * 2.0f;
            Vector3 nextPos = startPos + velocity * t + 0.5f * Physics.gravity * t * t;
            lineRenderer.SetPosition(i, nextPos);
        }
    }
}