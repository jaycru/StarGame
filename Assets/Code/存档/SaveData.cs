using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string saveId;
    public string saveName;
    public string sceneName;
    public string saveTime;

    public PlayerSaveData player;
    public List<InventorySaveItem> inventory = new List<InventorySaveItem>();
    public List<LootContainerSaveData> lootContainers = new List<LootContainerSaveData>();
}

[Serializable]
public class PlayerSaveData
{
    public float x;
    public float y;
    public float z;
    public float rotY;
    public int hp;
}

[Serializable]
public class InventorySaveItem
{
    public string itemId;
    public int amount;
}

[Serializable]
public class LootContainerSaveData
{
    public string containerId;
    public List<InventorySaveItem> items = new List<InventorySaveItem>();
}

