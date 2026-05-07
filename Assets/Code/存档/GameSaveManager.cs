using System;
using System.Collections;
using System.Collections.Generic;
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

        data.lootContainers = CreateLootContainerSaveData();

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

        ApplyLootContainerSaveData(data.lootContainers);

        if (GameSceneManager.instance != null)
        {
            GameSceneManager.instance.SetGamePlayActive(false, null, null);
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

    private List<LootContainerSaveData> CreateLootContainerSaveData()
    {
        List<LootContainerSaveData> result = new List<LootContainerSaveData>();
        Interactable[] containers = FindObjectsOfType<Interactable>(true);

        foreach (Interactable container in containers)
        {
            if (container == null || container.type != InteractType.LootList)
            {
                continue;
            }

            LootContainerSaveData containerData = new LootContainerSaveData
            {
                containerId = container.GetSaveId(),
                items = new List<InventorySaveItem>()
            };

            foreach (LootItem lootItem in container.itemsInObject)
            {
                if (lootItem == null || lootItem.details == null || lootItem.amount <= 0)
                {
                    continue;
                }

                string itemId = GetItemSaveId(lootItem.details);
                if (string.IsNullOrWhiteSpace(itemId))
                {
                    Debug.LogWarning("Loot save skipped an item with empty itemId in container: " + containerData.containerId);
                    continue;
                }

                containerData.items.Add(new InventorySaveItem
                {
                    itemId = itemId,
                    amount = lootItem.amount
                });
            }

            result.Add(containerData);
        }

        return result;
    }

    private void ApplyLootContainerSaveData(List<LootContainerSaveData> savedContainers)
    {
        if (savedContainers == null)
        {
            return;
        }

        Dictionary<string, LootContainerSaveData> savedById = new Dictionary<string, LootContainerSaveData>();
        foreach (LootContainerSaveData savedContainer in savedContainers)
        {
            if (savedContainer == null || string.IsNullOrWhiteSpace(savedContainer.containerId))
            {
                continue;
            }

            savedById[savedContainer.containerId] = savedContainer;
        }

        Interactable[] containers = FindObjectsOfType<Interactable>(true);
        foreach (Interactable container in containers)
        {
            if (container == null || container.type != InteractType.LootList)
            {
                continue;
            }

            if (!savedById.TryGetValue(container.GetSaveId(), out LootContainerSaveData savedContainer))
            {
                continue;
            }

            container.itemsInObject.Clear();

            if (savedContainer.items == null)
            {
                continue;
            }

            foreach (InventorySaveItem savedItem in savedContainer.items)
            {
                if (savedItem == null || savedItem.amount <= 0)
                {
                    continue;
                }

                ItemAsset asset = FindItemById(savedItem.itemId);
                if (asset == null)
                {
                    Debug.LogWarning("Loot load skipped missing itemId: " + savedItem.itemId);
                    continue;
                }

                container.itemsInObject.Add(new LootItem
                {
                    details = asset,
                    amount = savedItem.amount
                });
            }
        }
    }

    private ItemAsset FindItemById(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        ItemAsset[] allItems = Resources.LoadAll<ItemAsset>("");
        foreach (ItemAsset item in allItems)
        {
            if (item == null)
            {
                continue;
            }

            if (GetItemSaveId(item) == itemId || item.name == itemId)
            {
                return item;
            }
        }

        return null;
    }

    private string GetItemSaveId(ItemAsset asset)
    {
        if (asset == null)
        {
            return string.Empty;
        }

        return string.IsNullOrWhiteSpace(asset.itemId) ? asset.name : asset.itemId;
    }
}
