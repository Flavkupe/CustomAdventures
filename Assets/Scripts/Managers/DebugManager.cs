using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class DebugManager : SingletonObject<DebugManager>
{
    public bool DebugMode = false;

    public LogTraceType[] EnabledLogs;

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
}

public enum LogTraceType
{
    State,
}
