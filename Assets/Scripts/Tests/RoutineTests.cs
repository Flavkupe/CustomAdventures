using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;

public class RoutineTests : IPrebuildSetup
{
    public void Setup()
    {
        Log.SetLogger(new TestLogger());
    }

    private class ValContainer
    {
        public int Val = 0;
    }

    private IEnumerator AddTimes(int times, ValContainer container)
    {
        for (var i = 0; i < times; i++)
        {
            yield return container.Val++;
        }
    }

    private IEnumerator IncrementThenThrowsException(ValContainer container)
    {
        yield return null;
        container.Val++;
        yield return null;
        throw new Exception("Ex");
    }

    [UnityTest]
    public IEnumerator RoutineBasic_CallsInnerCoroutine_StopsExecution()
    {
        var container = new ValContainer();
        var routine = new Routine(() => AddTimes(3, container));
        yield return routine;
        Assert.AreEqual(3, container.Val);
    }

    [UnityTest]
    public IEnumerator Then_RunsThenAction()
    {
        var container = new ValContainer();
        var didThing = false;
        var routine = new Routine(() => AddTimes(3, container));
        routine.Then(() => didThing = true);
        yield return routine;
        Assert.AreEqual(3, container.Val);
        Assert.IsTrue(didThing);
    }

    [UnityTest]
    public IEnumerator Then_RunsThenRoutine()
    {
        var container = new ValContainer();
        var routine = new Routine(() => AddTimes(3, container));
        routine.Then(new Routine(() => AddTimes(3, container)));
        yield return routine;
        Assert.AreEqual(6, container.Val);
    }

    [UnityTest]
    public IEnumerator Then_ChainMultipleThens()
    {
        var container = new ValContainer();
        var count = 0;
        var routine = new Routine(() => AddTimes(3, container));
        routine.Then(new Routine(() => AddTimes(3, container)));
        routine.Then(new Routine(() => AddTimes(3, container)));
        routine.Then(() => new Routine(() => AddTimes(3, container)));
        routine.Then(() => count++);
        routine.Then(() => count += 2);
        routine.Then(() => count++);
        yield return routine;
        Assert.AreEqual(12, container.Val);
        Assert.AreEqual(4, count);
    }

    [UnityTest]
    public IEnumerator Then_DaisyChainThens()
    {
        var container = new ValContainer();
        var count = 0;
        var routine = new Routine(() => AddTimes(3, container));
        routine.Then(new Routine(() => AddTimes(3, container)))
            .Then(new Routine(() => AddTimes(3, container)))
            .Then(() => new Routine(() => AddTimes(3, container)))
            .Then(() => count++)
            .Then(() => count += 2)
            .Then(() => count++);
        yield return routine;
        Assert.AreEqual(12, container.Val);
        Assert.AreEqual(4, count);
    }

    [UnityTest]
    public IEnumerator Finally_TriggersWithoutThens()
    {
        var container = new ValContainer();
        var routine = new Routine(() => AddTimes(3, container));
        routine.Finally(() => container.Val++);
        yield return routine;
        Assert.AreEqual(4, container.Val);
    }

    [UnityTest]
    public IEnumerator Finally_MultipleStack()
    {
        var container = new ValContainer();
        var routine = new Routine(() => AddTimes(3, container));
        routine.Finally(() => container.Val++);
        routine.Finally(() => container.Val++);
        routine.Finally(() => container.Val++);
        yield return routine;
        Assert.AreEqual(6, container.Val);
    }

    [UnityTest]
    public IEnumerator OnCompleted_TriggersAtEnd()
    {
        var container = new ValContainer();
        var routine = new Routine(() => AddTimes(3, container));
        routine.OnCompleted(() => container.Val++);
        yield return routine;
        Assert.AreEqual(4, container.Val);
    }

    [UnityTest]
    public IEnumerator OnCompleted_TriggersWithFinallyAndThen()
    {
        var container = new ValContainer();
        var routine = new Routine(() => AddTimes(3, container));
        routine.OnCompleted(() => container.Val++);
        routine.Finally(() => container.Val++);
        routine.Then(() => container.Val++);
        yield return routine;
        Assert.AreEqual(6, container.Val);
    }

    [UnityTest]
    public IEnumerator OnCatch_ExceptionCaught()
    {
        LogAssert.ignoreFailingMessages = true;
        var container = new ValContainer();
        var routine = new Routine(() => IncrementThenThrowsException(container));
        Exception exception = null;
        routine.Catch((ex) =>
        {
            exception = ex;
        });

        yield return routine;

        Assert.AreEqual(1, container.Val);
        Assert.AreEqual("Ex", exception.Message);
    }

    [UnityTest]
    public IEnumerator Finally_HappensOnException()
    {
        LogAssert.ignoreFailingMessages = true;
        var container = new ValContainer();
        var routine = new Routine(() => IncrementThenThrowsException(container));
        routine.Finally(() => container.Val++);
        routine.Finally(() => container.Val++);
        routine.Finally(() => container.Val++);
        Exception exception = null;
        routine.Catch((ex) =>
        {
            exception = ex;
        });

        yield return routine;

        Assert.AreEqual(4, container.Val);
        Assert.AreEqual("Ex", exception.Message);
    }
}
