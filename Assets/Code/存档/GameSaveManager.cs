using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance;

    [Header("Save Targets")]
    [SerializeField] private Transform player;

    private const string GameSceneName = "GameScene";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneLoadedCallback()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureInstanceForScene(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureInstanceForScene(scene);
    }

    private static void EnsureInstanceForScene(Scene scene)
    {
        if (scene.name != GameSceneName || Instance != null)
        {
            return;
        }

        GameObject saveManagerObject = new GameObject("GameSaveManager");
        saveManagerObject.AddComponent<GameSaveManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        FindPlayerIfMissing();
    }

    private void Start()
    {
        FindPlayerIfMissing();

        if (LoadRequest.isLoadingSave)
        {
            StartCoroutine(LoadRequestedSaveNextFrame());
        }
    }

    private IEnumerator LoadRequestedSaveNextFrame()
    {
        yield return null;

        SaveData data = SaveSystem.LoadGame(LoadRequest.saveId);

        if (data != null)
        {
            ApplySaveData(data);
        }

        LoadRequest.Clear();
    }

    public void SaveCurrentGame()
    {
        SaveData data = CreateSaveData();
        SaveSystem.SaveGame(data);
    }

    private SaveData CreateSaveData()
    {
        FindPlayerIfMissing();

        SaveData data = new SaveData
        {
            saveId = SaveSystem.GenerateSaveId(),
            saveName = "存档 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            sceneName = SceneManager.GetActiveScene().name,
            player = new PlayerSaveData()
        };

        if (player != null)
        {
            data.player.x = player.position.x;
            data.player.y = player.position.y;
            data.player.z = player.position.z;
            data.player.rotY = player.eulerAngles.y;

            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                data.player.hp = health.CurrentHP;
            }
        }
        else
        {
            Debug.LogWarning("Save warning: player transform was not found.");
        }

        if (InventoryManager.Instance != null)
        {
            data.inventory = InventoryManager.Instance.CreateInventorySaveData();
        }

        return data;
    }

    private void ApplySaveData(SaveData data)
    {
        FindPlayerIfMissing();

        if (data == null)
        {
            return;
        }

        if (player != null && data.player != null)
        {
            CharacterController characterController = player.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            player.position = new Vector3(data.player.x, data.player.y, data.player.z);
            player.eulerAngles = new Vector3(0f, data.player.rotY, 0f);

            if (characterController != null)
            {
                characterController.enabled = true;
            }

            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.SetHealth(data.player.hp);
            }
        }
        else
        {
            Debug.LogWarning("Load warning: player transform was not found.");
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.LoadInventory(data.inventory);
        }

        if (GameSceneManager.instance != null)
        {
            GameSceneManager.instance.ForceGamePlayMode();
        }
    }

    private void FindPlayerIfMissing()
    {
        if (player != null)
        {
            return;
        }

        if (PlayerHealth.Instance != null)
        {
            player = PlayerHealth.Instance.transform;
            return;
        }

        GameObject taggedPlayer = GameObject.FindGameObjectWithTag("Player");
        if (taggedPlayer != null)
        {
            player = taggedPlayer.transform;
        }
    }
}
