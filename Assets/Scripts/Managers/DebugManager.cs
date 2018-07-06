using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class DebugManager : SingletonObject<DebugManager>
{
    public bool DebugMode = false;

    public LogTraceType[] EnabledLogs;

    public ItemCardData ItemToGive;

    public void Log(string message, LogTraceType type)
    {
        if (EnabledLogs.Contains(type))
        {
            Debug.Log(message);
        }
    }

    [UsedImplicitly]
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (DebugMode)
        {
            if (Input.GetKeyUp(KeyCode.P) && ItemToGive != null)
            {
                var item = ItemToGive.CreateInventoryItem();
                Game.Player.Inventory.TryMoveToInventory(item, true);
            }
        }
    }
}

public enum LogTraceType
{
    State,
}
