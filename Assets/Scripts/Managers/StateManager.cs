using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class StateManager : SingletonObject<StateManager>
{
    public bool IsPaused = false;

    public float MousedownSpeedMultiplier = 3.0f;

    private readonly Dictionary<GameState, Queue<Action>> _onStateChangeFromQueues = new Dictionary<GameState, Queue<Action>>();
    private readonly Dictionary<GameState, Queue<Action>> _onStateChangeToQueues = new Dictionary<GameState, Queue<Action>>();
    private readonly Dictionary<GameState, Queue<Func<IEnumerator>>> _onStateChangeFromCoroutineQueues = new Dictionary<GameState, Queue<Func<IEnumerator>>>();
    private readonly Dictionary<GameState, Queue<Func<IEnumerator>>> _onStateChangeToCoroutineQueues = new Dictionary<GameState, Queue<Func<IEnumerator>>>();

    private readonly Dictionary<TriggeredEvent, Queue<Func<IEnumerator>>> _onTriggeredEventCoroutineQueues = new Dictionary<TriggeredEvent, Queue<Func<IEnumerator>>>();
    private readonly Dictionary<TriggeredEvent, Queue<Action>> _onTriggeredEventActionQueues = new Dictionary<TriggeredEvent, Queue<Action>>();

    private readonly Queue<Func<Routine>> _routineQueue = new Queue<Func<Routine>>();

    public HashSet<Routine> CoroutineTraces = new HashSet<Routine>();

    //private string Traces
    //{
    //    get { return string.Join("\n\n\n", CoroutineTraces.Select(a => a.Trace).ToArray()); }
    //}

    private bool _processingCoroutine;

    private void EnqueueThingOnQueue<TR, T>(TR state, T thing, Dictionary<TR, Queue<T>> dict)
    {
        dict[state].Enqueue(thing);
    }

    public void EnqueueRoutine(Routine routine)
    {
        _routineQueue.Enqueue(() => routine);
    }

    public void EnqueueCoroutine(IEnumerator enumerator)
    {
        EnqueueCoroutine(() => enumerator);
    }

    public void EnqueueCoroutine(Func<IEnumerator> action)
    {
        _routineQueue.Enqueue(() => Routine.Create(action));
    }

    public void EnqueueCoroutine(Func<Routine> action)
    {
        _routineQueue.Enqueue(action);
    }

    public void EnqueueIfNotState(GameState afterStateChangedFrom, Routine routine)
    {
        EnqueueIfNotState(afterStateChangedFrom, () => routine);
    }

    public void EnqueueIfNotState(GameState afterStateChangedFrom, Func<IEnumerator> action)
    {
        if (afterStateChangedFrom == State)
        {
            EnqueueThingOnQueue(afterStateChangedFrom, action, _onStateChangeFromCoroutineQueues);
        }
        else
        {
            // State has already different; enqueue action to happen asap
            EnqueueCoroutine(action);
        }
    }

    public void EnqueueOnNewState(GameState afterStateChangedTo, Func<IEnumerator> action)
    {
        if (afterStateChangedTo != State)
        {
            EnqueueThingOnQueue(afterStateChangedTo, action, _onStateChangeToCoroutineQueues);
        }
        else
        {
            // State already at required state; enqueue action to happen asap
            EnqueueCoroutine(action);
        }
    }

    public void EnqueueIfNotState(GameState afterStateChangedFrom, Action action)
    {
        if (afterStateChangedFrom == State)
        {
            EnqueueThingOnQueue(afterStateChangedFrom, action, _onStateChangeFromQueues);
        }
        else
        {
            // State has already different; perform action now
            action();
        }
    }

    public void EnqueueOnNewState(GameState afterStateChangedTo, Action action)
    {
        // TODO: make this case conditional on params...?
        if (afterStateChangedTo != State)
        {
            EnqueueThingOnQueue(afterStateChangedTo, action, _onStateChangeToQueues);
        }
        else
        {
            // State already at required state; perform action now
            action();
        }
    }

    public void EnqueueOnNewState(GameState afterStateChangedTo, Routine routine)
    {
        EnqueueOnNewState(afterStateChangedTo, () => routine);
    }

    public void EnqueueTriggeredEventAction(TriggeredEvent trigger, Action action)
    {
        EnqueueThingOnQueue(trigger, action, _onTriggeredEventActionQueues);
    }

    public void EnqueueTriggeredEventCoroutine(TriggeredEvent trigger, Func<IEnumerator> action)
    {
        EnqueueThingOnQueue(trigger, action, _onTriggeredEventCoroutineQueues);
    }

    private GameState _state;
    private GameState _lastState;

    public GameState State { get { return _state; } }

    public bool DebugEnabled = false;

    public void SetState(GameState state)
    {
        if (state != _state)
        {            
            OnStateChanged(_state, state);
            _lastState = _state;
            _state = state;
            Game.UI.UpdateUI();
        }    
    }

    public void RevertState()
    {
        SetState(_lastState);
    }

    public void TriggerEvent(TriggeredEvent triggeredEvent)
    {
        ProcessQueuedActions(_onTriggeredEventActionQueues[triggeredEvent]);
        MoveCoroutinesToInvokeQueue(_onTriggeredEventCoroutineQueues[triggeredEvent]);
    }

    private void OnStateChanged(GameState oldState, GameState newState)
    {
        ProcessQueuedActions(_onStateChangeToQueues[newState]);
        ProcessQueuedActions(_onStateChangeFromQueues[oldState]);
        MoveCoroutinesToInvokeQueue(_onStateChangeToCoroutineQueues[newState]);
        MoveCoroutinesToInvokeQueue(_onStateChangeFromCoroutineQueues[oldState]);
    }

    /// <summary>
    /// Transfers coroutines from special queue to main coroutine queue, which
    /// will actually be invoking each of the coroutines in sequence when the
    /// time is right to do so.
    /// </summary>
    private void MoveCoroutinesToInvokeQueue(Queue<Func<IEnumerator>> queue)
    {
        while (queue.Count > 0)
        {
            Func<IEnumerator> action = queue.Dequeue();
            EnqueueCoroutine(action);
        }
    }

    /// <summary>
    /// Processes next queued routine if any are in the queue
    /// </summary>
    private IEnumerator InvokeNextQueuedCoroutine()
    {
        if (_routineQueue.Count > 0)
        {
            Func<Routine> action = _routineQueue.Dequeue();
            _processingCoroutine = true;
            yield return action();
            _processingCoroutine = false;
        }
    }

    private void ProcessQueuedActions(Queue<Action> queue)
    {
        while (queue.Count > 0)
        {
            Action action = queue.Dequeue();
            action();
        }
    }

    [UsedImplicitly]
    private void Awake()
    {
        Instance = this;
        foreach (GameState state in Enum.GetValues(typeof(GameState)))
        {
            _onStateChangeFromQueues[state] = new Queue<Action>();
            _onStateChangeToQueues[state] = new Queue<Action>();
            _onStateChangeFromCoroutineQueues[state] = new Queue<Func<IEnumerator>>();
            _onStateChangeToCoroutineQueues[state] = new Queue<Func<IEnumerator>>();
        }

        foreach (TriggeredEvent trigger in Enum.GetValues(typeof(TriggeredEvent)))
        {
            _onTriggeredEventCoroutineQueues[trigger] = new Queue<Func<IEnumerator>>();
            _onTriggeredEventActionQueues[trigger] = new Queue<Action>();
        }
    }

    [UsedImplicitly]
    private void Update ()
    {
        if (!_processingCoroutine && _routineQueue.Count > 0)
        {
            StartRoutine(InvokeNextQueuedCoroutine());
        }
	}

    public float GetMouseDownSpeedMultiplier()
    {
        return Input.GetMouseButton(0) ? MousedownSpeedMultiplier : 1.0f;
    }

    public static class Checks
    {
        private static bool IsState(params GameState[] states)
        {
            return states.Contains(Instance.State);
        }

        public static bool CanPlayerWalk { get { return IsState(GameState.AwaitingCommand); } }
    }
}

public enum TriggeredEvent
{
    EntityTurnDone,
    CardDrawDone,
    CardDrawConfirmed,
}

public enum GameState
{
    Neutral,
    DrawingCard,
    AwaitingCommand,
    CharacterMoving,    
    EnemyTurn,
    AwaitingSelection,
    CharacterActing,
}