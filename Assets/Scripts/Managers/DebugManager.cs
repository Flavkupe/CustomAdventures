using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    void Awake()
    {
        Instance = this;
    }
}

public enum LogTraceType
{
    State,
}
