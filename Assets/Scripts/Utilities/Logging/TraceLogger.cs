using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum TraceType
{
    Behavior,
}


public static class Trace
{
    private static readonly HashSet<TraceType> _blacklist = new HashSet<TraceType>();

    public static void EnableTrace(TraceType type)
    {
        _blacklist.Remove(type);
    }

    public static void DisableTrace(TraceType type)
    {
        _blacklist.Add(type);
    }

    public static void Info(TraceType type, string message)
    {
        if (!_blacklist.Contains(type))
        {
            Log.Info(message);
        }
    }
}

