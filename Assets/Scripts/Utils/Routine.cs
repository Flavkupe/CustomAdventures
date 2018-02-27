
using System;
using System.Collections;
using System.Collections.Generic;

public class Routine : IEnumerator
{
    private Func<IEnumerator> _func;
    private Routine _next = null;
    private Action _thenAction = null;
    private IEnumerator _current = null;

    public event EventHandler Completed;

    private bool _executed = false;

    protected Routine()
    {
    }

    public Routine(Func<IEnumerator> func)
    {
        _func = func;
    }

    public object Current
    {
        get { return _current ?? (_next != null ? _next.Current : null); }
    }

    public virtual IEnumerator Execute()
    {
        return _func();
    }

    public bool MoveNext()
    {
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

        if (_thenAction != null)
        {
            _thenAction();
            _thenAction = null;
        }

        if (_next != null)
        {
            return _next.MoveNext();
        }

        if (Completed != null)
        {
            this.Completed(this, null);
        }

        return false;
    }

    public void Reset()
    {
        // TODO
    }

    /// <summary>
    /// Queues an Action callback to happen after the Routine completes.
    /// If a Routine is also hooked via Then, this Action will happen first.
    /// Returns this object for chaining.
    /// </summary>
    public Routine Then(Action action)
    {
        _thenAction = action;
        return this;
    }

    /// <summary>
    /// Queues a Routine to happen after this Routine completes. Only one of these
    /// can be set per Routine, but they can be chained by calling this function on the returned
    /// Routine reference.
    /// </summary>
    /// <param name="next">Which Routine to run after this Routine completes.</param>
    /// <returns>A reference to the provided parameter, to ease chaining.</returns>
    public Routine Then(Routine next)
    {
        _next = next;
        return _next;
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
}

public class Routine<T> : Routine
{
    private Func<T, IEnumerator> _func;
    private T _arg1;

    public Routine(Func<T, IEnumerator> func, T arg1)
    {
        _func = func;
        _arg1 = arg1;
    }

    public override IEnumerator Execute()
    {
        return _func(_arg1);
    }
}

public class Routine<T1, T2> : Routine
{
    private Func<T1, T2, IEnumerator> _func;
    private T1 _arg1;
    private T2 _arg2;

    public Routine(Func<T1, T2, IEnumerator> func, T1 arg1, T2 arg2)
    {
        _func = func;
        _arg1 = arg1;
        _arg2 = arg2;
    }

    public override IEnumerator Execute()
    {
        return _func(_arg1, _arg2);
    }
}

public class Routine<T1, T2, T3> : Routine
{
    private Func<T1, T2, T3, IEnumerator> _func;
    private T1 _arg1;
    private T2 _arg2;
    private T3 _arg3;

    public Routine(Func<T1, T2, T3, IEnumerator> func, T1 arg1, T2 arg2, T3 arg3)
    {
        _func = func;
        _arg1 = arg1;
        _arg2 = arg2;
        _arg3 = arg3;
    }

    public override IEnumerator Execute()
    {
        return _func(_arg1, _arg2, _arg3);
    }
}

public class RoutineChain : IEnumerator
{
    private Queue<Routine> _queue = new Queue<Routine>();

    private Action _then = null;

    private Routine _current = null;

    public RoutineChain()
    {
    }

    public RoutineChain(params Routine[] routines)
    {
        foreach (Routine routine in routines)
        {
            _queue.Enqueue(routine);
        }
    }

    public void Enqueue(Routine routine)
    {
        _queue.Enqueue(routine);
    }

    public void Then(Action action)
    {
        _then = action;
    }

    public object Current { get { return _current; } }

    public bool MoveNext()
    {
        if (_queue.Count == 0)
        {
            _current = null;
            if (_then != null)
            {
                _then();
            }

            return false;
        }

        _current = _queue.Dequeue();
        return true;
    }

    public void Reset()
    {
    }
}
