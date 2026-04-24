using UnityEngine;

public class Chat : MonoBehaviour
{
    [Header("Chat")]
    public TextAsset chatText;

    [Header("Front Check")]
    [SerializeField] private bool requirePlayerInFront = true;
    [SerializeField, Range(1f, 180f)] private float frontAngle = 120f;
    [SerializeField] private Transform facingReference;
    [SerializeField] private bool invertForward;

    private const string PlayerTag = "Player";

    private Transform askPanel;
    private BeginChat beginChatButton;
    private Transform currentPlayer;

    private void Start()
    {
        GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
        if (inventoryCanvas == null)
        {
            Debug.LogWarning("Chat setup failed: InventoryCanvas was not found.", this);
            return;
        }

        if (inventoryCanvas.transform.childCount <= 2)
        {
            Debug.LogWarning("Chat setup failed: InventoryCanvas does not contain AskPanel at child index 2.", this);
            return;
        }

        askPanel = inventoryCanvas.transform.GetChild(2);
        beginChatButton = askPanel.GetChild(0).GetComponent<BeginChat>();
        HideAskPanel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerTag)) return;

        currentPlayer = other.transform;
        RefreshAskPanel();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(PlayerTag)) return;

        currentPlayer = other.transform;
        RefreshAskPanel();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(PlayerTag)) return;

        if (currentPlayer == other.transform)
        {
            currentPlayer = null;
        }

        HideAskPanel();
    }

    private void OnDisable()
    {
        HideAskPanel();
    }

    private void RefreshAskPanel()
    {
        if (askPanel == null || currentPlayer == null) return;

        if (!IsPlayerInFront(currentPlayer))
        {
            HideAskPanel();
            return;
        }

        askPanel.gameObject.SetActive(true);

        Transform button = askPanel.GetChild(0);
        button.gameObject.SetActive(true);

        if (beginChatButton == null)
        {
            beginChatButton = button.GetComponent<BeginChat>();
        }

        if (beginChatButton != null)
        {
            beginChatButton.SetText(chatText);
        }
    }

    private void HideAskPanel()
    {
        if (askPanel != null)
        {
            askPanel.gameObject.SetActive(false);
        }
    }

    private bool IsPlayerInFront(Transform playerTransform)
    {
        if (!requirePlayerInFront) return true;

        Vector3 toPlayer = playerTransform.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.0001f) return true;

        Transform reference = facingReference != null ? facingReference : transform;
        Vector3 forward = invertForward ? -reference.forward : reference.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f) return true;

        float minDot = Mathf.Cos(frontAngle * 0.5f * Mathf.Deg2Rad);
        return Vector3.Dot(forward.normalized, toPlayer.normalized) >= minDot;
    }
}
