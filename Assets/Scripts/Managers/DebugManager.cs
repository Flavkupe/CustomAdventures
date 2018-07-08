using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class DebugManager : SingletonObject<DebugManager>
{
    public bool DebugMode = false;

    public LogTraceType[] EnabledLogs;

    public ItemCardData[] ItemsToGive;

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
            if (Input.GetKeyUp(KeyCode.P) && ItemsToGive.Length > 0)
            {
                foreach (var template in ItemsToGive)
                {
                    var item = template.CreateInventoryItem();
                    if (!Game.Player.Inventory.TryMoveToInventory(item, true, false))
                    {
                        Game.Player.Inventory.DiscardItem(item, false);
                    }
                }
            }
        }
    }
}

public enum LogTraceType
{
    State,
}
