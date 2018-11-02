using System;
using UnityEngine;
using UnityEngine.TestTools;

public interface ILogger
{
    void Warning(string warning);

    void Error(string error);

    void Error(Exception ex);

    void Info(string info);
}

public class DefaultLogger : ILogger
{
    public void Warning(string warning)
    {
        Debug.LogWarning(warning);
    }

    public void Error(string error)
    {
        Debug.LogError(error);
    }

    public void Error(Exception ex)
    {
        Debug.LogException(ex);
    }

    public void Info(string info)
    {
        Debug.Log(info);
    }
}

public class TestLogger : ILogger
{
    public void Warning(string warning)
    {
        LogAssert.Expect(LogType.Warning, warning);
    }

    public void Error(string error)
    {
        LogAssert.Expect(LogType.Error, error);
    }

    public void Error(Exception ex)
    {
        LogAssert.Expect(LogType.Exception, ex.Message);
    }

    public void Info(string info)
    {
        LogAssert.Expect(LogType.Log, info);
    }
}

public class Log
{
    private static ILogger _logger = new DefaultLogger();

    public static void SetLogger(ILogger newLogger)
    {
        _logger = newLogger;
    }

    public static void Warning(string warning)
    {
        _logger.Warning(warning);
    }

    public static void Error(string error)
    {
        _logger.Error(error);
    }

    public static void Error(Exception ex)
    {
        _logger.Error(ex);
    }

    public static void Info(string info)
    {
        _logger.Info(info);
    }
}
