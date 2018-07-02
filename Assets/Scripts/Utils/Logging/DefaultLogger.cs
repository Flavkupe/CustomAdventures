using System;
using UnityEngine;
using UnityEngine.TestTools;

public interface ILogger
{
    void Warning(string warning);

    void Error(string error);

    void Error(Exception ex);
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
}

