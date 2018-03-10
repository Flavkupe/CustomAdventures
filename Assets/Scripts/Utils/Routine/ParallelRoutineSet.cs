using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ParallelRoutineSet : IEnumerator
{
    private HashSet<Routine> _routines = new HashSet<Routine>();
    private IEnumerator _func = null;
    private Func<IEnumerator, IEnumerator> _startCoroutineFunc;
    private int _running = 0;

    public ParallelRoutineSet(Func<IEnumerator, IEnumerator> startCoroutineFunc, params Routine[] routines)
    {
        _startCoroutineFunc = startCoroutineFunc;
        _routines.UnionWith(routines);
    }

    public ParallelRoutineSet(Func<IEnumerator, IEnumerator> startCoroutineFunc, params Func<IEnumerator>[] routines)
    {
        _startCoroutineFunc = startCoroutineFunc;
        _routines.UnionWith(routines.Select(a => Routine.Create(a)));
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

