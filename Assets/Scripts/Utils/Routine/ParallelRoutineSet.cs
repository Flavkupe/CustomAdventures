using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallelRoutineSet : IEnumerator, IRoutineConvertable
{
    private HashSet<Routine> _routines = new HashSet<Routine>();
    private IEnumerator _func = null;
    private Func<IEnumerator, Coroutine> _startCoroutineFunc;
    private int _running = 0;

    public ParallelRoutineSet(Func<IEnumerator, Coroutine> startCoroutineFunc, params Routine[] routines)
    {
        _startCoroutineFunc = startCoroutineFunc;
        _routines.UnionWith(routines);
    }

    public ParallelRoutineSet(Func<IEnumerator, Coroutine> startCoroutineFunc, params Func<IEnumerator>[] routines)
    {
        _startCoroutineFunc = startCoroutineFunc;
        _routines.UnionWith(routines.Select(a => Routine.Create(a)));
    }

    public Routine AsRoutine()
    {
        return Routine.Create(() => this);
    }

    public object Current
    {
        get { return _func; }
    }

    public bool MoveNext()
    {
        if (_func == null)
        {
            _func = Execute();
            return true;
        }

        return false;
    }

    public void Reset()
    {
    }

    private IEnumerator Execute()
    {
        _running = _routines.Count;
        foreach (var routine in _routines)
        {
            routine.Finally(() => _running--);
            _startCoroutineFunc(routine);
        }

        while (_running > 0)
        {
            yield return null;
        }
    }
}

