using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    private Camera mainCamera;
    public static GameSceneManager instance;

    public TextAsset intoText;
    public GameObject chatText;

    private TextOut textOut;
    private CameraClearFlags originalClearFlags;
    private Color originalBackgroundColor;
    private int originalCullingMask;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (chatText != null)
        {
            textOut = chatText.GetComponent<TextOut>();
        }

        if (mainCamera != null)
        {
            originalClearFlags = mainCamera.clearFlags;
            originalBackgroundColor = mainCamera.backgroundColor;
            originalCullingMask = mainCamera.cullingMask;
        }

        if (LoadRequest.isLoadingSave)
        {
            ForceGamePlayMode();
        }
        else
        {
            StartIntoDialogue();
        }
    }

    private void StartIntoDialogue()
    {
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = Color.black;
            mainCamera.cullingMask = 0;
        }

        if (intoText != null)
        {
            SetGamePlayActive(true, intoText);
        }
        else
        {
            ForceGamePlayMode();
        }
    }

    public void SetGamePlayActive(bool set, TextAsset text)
    {
        if (set)
        {
            Time.timeScale = 0f;

            if (textOut != null && text != null)
            {
                textOut.SetText(text);
            }

            if (chatText != null)
            {
                if (chatText.transform.parent != null)
                {
                    chatText.transform.parent.gameObject.SetActive(true);
                }

                chatText.SetActive(true);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("Enter dialogue mode.");
        }
        else
        {
            ForceGamePlayMode();
            Debug.Log("Enter gameplay mode.");
        }
    }

    public void ForceGamePlayMode()
    {
        if (mainCamera != null)
        {
            mainCamera.clearFlags = originalClearFlags;
            mainCamera.backgroundColor = originalBackgroundColor;
            mainCamera.cullingMask = originalCullingMask;
        }

        if (chatText != null)
        {
            if (textOut != null)
            {
                textOut.AwakeBrother(false);
            }

            chatText.SetActive(false);

            if (chatText.transform.parent != null)
            {
                chatText.transform.parent.gameObject.SetActive(false);
            }
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
