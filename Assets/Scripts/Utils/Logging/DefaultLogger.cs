using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Log
{
    public static void Warning(string warning)
    {
        Debug.LogWarning(warning);
    }

    public static void Error(string error)
    {
        Debug.LogError(error);
    }

    public static void Error(Exception ex)
    {
        Debug.LogException(ex);
    }
}

