
using System;
using System.Collections;
using UnityEngine;

public class Routine : IEnumerator
{
    private Func<IEnumerator> _func;
    private Routine _next = null;
    private IEnumerator _current = null;

    private Action _reject = null;
    protected bool _rejected = false;

    private Action _finally = null;

    private Action _completed = null;

    private bool _executed = false;    

    protected Routine()
    {
    }

    public Action CancellationCallback
    {
        get { return () => _rejected = true; }
    }

    public Routine(Func<IEnumerator> func)
    {
        _func = func;
    }

    public object Current
    {
        get
        {
            return _current ?? (_next != null ? _next.Current : null);
        }
    }

    protected virtual IEnumerator Execute()
    {
        return _func();
    }

    public bool MoveNext()
    {
        if (_rejected)
        {
            if (_reject != null)
            {
                _reject();
            }
            if (_finally != null)
            {
                _finally();
            }

            return false;
        }

        if (!_executed)
        {
            _executed = true;
            _current = Execute();
            if (_current != null)
            {
                return true;
            }
        }
        else
        {
            _current = null;
        }

        if (_next != null)
        {
            if (_next.MoveNext())
            {
                return true;
            }
        }

        if (_completed != null)
        {
            _completed();
        }

        if (_finally != null)
        {
            _finally();
        }

        return false;
    }

    public void Reset()
    {
        // TODO
    }

    public void OnReject(Action action)
    {
        _reject = action;
    }

    public void OnCompleted(Action action)
    {
        _completed = action;
    }

    /// <summary>
    /// Queues an Action callback to happen after the Routine completes.
    /// If a Routine is also hooked via Then, this Action will happen first.
    /// Returns this object for chaining.
    /// </summary>
    public Routine Then(Action action)
    {
        return Then(Routine.Create(action));
    }

    public Routine Then(Func<IEnumerator> func)
    {
        return Then(Routine.Create(func));
    }

    /// <summary>
    /// Queues a Routine to happen after this Routine completes. Only one of these
    /// can be set per Routine, but they can be chained by calling this function on the returned
    /// Routine reference. If a "Then" has already been set for this Routine, the next routine
    /// with be appended to the end of the Routine chain.
    /// </summary>
    /// <param name="next">Which Routine to run after this Routine completes.</param>
    /// <returns>A reference to the provided parameter, to ease chaining.</returns>
    public Routine Then(Routine next)
    {
        if (_next == null)
        {
            _next = next;
            return _next;
        }
        else
        {
            return _next.Then(next);
        }
    }

    /// <summary>
    /// Runs at the end, no matter what (even if rejected)
    /// </summary>
    /// <param name="action"></param>
    public void Finally(Action action)
    {
        _finally = action;
    }

    private static IEnumerator DoActionQuick(Action action)
    {
        action();
        yield break;
    }

    private static IEnumerator DoActionQuick<T>(Action<T> action, T arg1)
    {
        action(arg1);
        yield break;
    }

    public static Routine Create(Action action)
    {
        return Routine.Create(() => DoActionQuick(action));
    }

    public static Routine Create<T>(Action<T> action, T arg1)
    {
        return Routine.Create(() => DoActionQuick(action, arg1));
    }

    public static Routine Create(Func<IEnumerator> func)
    {
        return new Routine(func);
    }

    public static Routine<T> Create<T>(Func<T, IEnumerator> func, T arg1)
    {
        return new Routine<T>(func, arg1);
    }

    public static Routine<T1, T2> Create<T1, T2>(Func<T1, T2, IEnumerator> func, T1 arg1, T2 arg2)
    {
        return new Routine<T1, T2>(func, arg1, arg2);
    }

    public static Routine<T1, T2, T3> Create<T1, T2, T3>(Func<T1, T2, T3, IEnumerator> func, T1 arg1, T2 arg2, T3 arg3)
    {
        return new Routine<T1, T2, T3>(func, arg1, arg2, arg3);
    }

    public static Routine<T1, T2, T3, T4> Create<T1, T2, T3, T4>(Func<T1, T2, T3, T4, IEnumerator> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        return new Routine<T1, T2, T3, T4>(func, arg1, arg2, arg3, arg4);
    }

    public static CancellableRoutine CreateCancellable(Func<Action, IEnumerator> func)
    {
        return new CancellableRoutine(func);
    }

    public static CancellableRoutine<T> CreateCancellable<T>(Func<Action, T, IEnumerator> func, T arg1)
    {
        return new CancellableRoutine<T>(func, arg1);
    }

    public static CancellableRoutine<T1, T2> CreateCancellable<T1, T2>(Func<Action, T1, T2, IEnumerator> func, T1 arg1, T2 arg2)
    {
        return new CancellableRoutine<T1, T2>(func, arg1, arg2);
    }

    public static IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}

public class Routine<T> : Routine
{
    private Func<T, IEnumerator> _func;
    protected T _arg1;

    public Routine(Func<T, IEnumerator> func, T arg1)
    {
        _func = func;
        _arg1 = arg1;
    }

    protected override IEnumerator Execute()
    {
        return _func(_arg1);
    }
}

public class Routine<T1, T2> : Routine
{
    private Func<T1, T2, IEnumerator> _func;
    protected T1 _arg1;
    protected T2 _arg2;

    public Routine(Func<T1, T2, IEnumerator> func, T1 arg1, T2 arg2)
    {
        _func = func;
        _arg1 = arg1;
        _arg2 = arg2;
    }

    protected override IEnumerator Execute()
    {
        return _func(_arg1, _arg2);
    }
}

public class Routine<T1, T2, T3> : Routine
{
    private Func<T1, T2, T3, IEnumerator> _func;
    protected T1 _arg1;
    protected T2 _arg2;
    protected T3 _arg3;

    public Routine(Func<T1, T2, T3, IEnumerator> func, T1 arg1, T2 arg2, T3 arg3)
    {
        _func = func;
        _arg1 = arg1;
        _arg2 = arg2;
        _arg3 = arg3;
    }

    protected override IEnumerator Execute()
    {
        return _func(_arg1, _arg2, _arg3);
    }
}

public class Routine<T1, T2, T3, T4> : Routine
{
    private Func<T1, T2, T3, T4, IEnumerator> _func;
    protected T1 _arg1;
    protected T2 _arg2;
    protected T3 _arg3;
    protected T4 _arg4;

    public Routine(Func<T1, T2, T3, T4, IEnumerator> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        _func = func;
        _arg1 = arg1;
        _arg2 = arg2;
        _arg3 = arg3;
        _arg4 = arg4;
    }

    protected override IEnumerator Execute()
    {
        return _func(_arg1, _arg2, _arg3, _arg4);
    }
}

public class CancellableRoutine : Routine<Action>
{
    public CancellableRoutine(Func<Action, IEnumerator> func)
        : base(func, null)
    {
        this._arg1 = CancellationCallback;
    }
}

public class CancellableRoutine<T> : Routine<Action, T>
{
    public CancellableRoutine(Func<Action, T, IEnumerator> func, T arg1)
        : base(func, null, arg1)
    {
        this._arg1 = CancellationCallback;
    }
}

public class CancellableRoutine<T1, T2> : Routine<Action, T1, T2>
{
    public CancellableRoutine(Func<Action, T1, T2, IEnumerator> func, T1 arg1, T2 arg2)
        : base(func, null, arg1, arg2)
    {
        this._arg1 = CancellationCallback;
    }
}
