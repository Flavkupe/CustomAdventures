using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : SingletonObject<StateManager>
{
    public bool IsPaused = false;

    public float MousedownSpeedMultiplier = 3.0f;

    private Dictionary<GameState, Queue<Action>> onStateChangeFromQueues = new Dictionary<GameState, Queue<Action>>();
    private Dictionary<GameState, Queue<Action>> onStateChangeToQueues = new Dictionary<GameState, Queue<Action>>();
    private Dictionary<GameState, Queue<Func<IEnumerator>>> onStateChangeFromCoroutineQueues = new Dictionary<GameState, Queue<Func<IEnumerator>>>();
    private Dictionary<GameState, Queue<Func<IEnumerator>>> onStateChangeToCoroutineQueues = new Dictionary<GameState, Queue<Func<IEnumerator>>>();

    private Dictionary<TriggeredEvent, Queue<Func<IEnumerator>>> onTriggeredEventCoroutineQueues = new Dictionary<TriggeredEvent, Queue<Func<IEnumerator>>>();
    private Dictionary<TriggeredEvent, Queue<Action>> onTriggeredEventActionQueues = new Dictionary<TriggeredEvent, Queue<Action>>();

    private Queue<Func<IEnumerator>> coroutineQueue = new Queue<Func<IEnumerator>>();

    private bool processingCoroutine = false;

    private void EnqueueThingOnQueue<R, T>(R state, T thing, Dictionary<R, Queue<T>> dict)
    {
        dict[state].Enqueue(thing);
    }

    public void EnqueueRoutine(Routine routine)
    {
        this.coroutineQueue.Enqueue(() => routine);
    }

    public void EnqueueCoroutine(Func<IEnumerator> action)
    {
        this.coroutineQueue.Enqueue(action);
    }

    public void EnqueueIfNotState(GameState afterStateChangedFrom, Routine routine)
    {
        EnqueueIfNotState(afterStateChangedFrom, () => routine);
    }

    public void EnqueueIfNotState(GameState afterStateChangedFrom, Func<IEnumerator> action)
    {
        if (afterStateChangedFrom == this.State)
        {
            EnqueueThingOnQueue(afterStateChangedFrom, action, onStateChangeFromCoroutineQueues);
        }
        else
        {
            // State has already different; enqueue action to happen asap
            EnqueueCoroutine(action);
        }
    }

    public void EnqueueOnNewState(GameState afterStateChangedTo, Func<IEnumerator> action)
    {
        if (afterStateChangedTo != this.State)
        {
            EnqueueThingOnQueue(afterStateChangedTo, action, onStateChangeToCoroutineQueues);
        }
        else
        {
            // State already at required state; enqueue action to happen asap
            EnqueueCoroutine(action);
        }
    }

    public void EnqueueIfNotState(GameState afterStateChangedFrom, Action action)
    {
        if (afterStateChangedFrom == this.State)
        {
            EnqueueThingOnQueue(afterStateChangedFrom, action, onStateChangeFromQueues);
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
        if (afterStateChangedTo != this.State)
        {
            EnqueueThingOnQueue(afterStateChangedTo, action, onStateChangeToQueues);
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
        EnqueueThingOnQueue(trigger, action, onTriggeredEventActionQueues);
    }

    public void EnqueueTriggeredEventCoroutine(TriggeredEvent trigger, Func<IEnumerator> action)
    {
        EnqueueThingOnQueue(trigger, action, onTriggeredEventCoroutineQueues);
    }

    private GameState _state;
    private GameState _lastState;

    public GameState State { get { return _state; } }

    public void SetState(GameState state)
    {
        if (state != _state)
        {            
            OnStateChanged(_state, state);
            _lastState = _state;
            _state = state;
            UIManager.Instance.UpdateUI();
        }    
    }

    public void RevertState()
    {
        this.SetState(_lastState);
    }

    public void TriggerEvent(TriggeredEvent triggeredEvent)
    {
        ProcessQueuedActions(onTriggeredEventActionQueues[triggeredEvent]);
        MoveCoroutinesToInvokeQueue(onTriggeredEventCoroutineQueues[triggeredEvent]);
    }

    private void OnStateChanged(GameState oldState, GameState newState)
    {
        ProcessQueuedActions(onStateChangeToQueues[newState]);
        ProcessQueuedActions(onStateChangeFromQueues[oldState]);
        MoveCoroutinesToInvokeQueue(onStateChangeToCoroutineQueues[newState]);
        MoveCoroutinesToInvokeQueue(onStateChangeFromCoroutineQueues[oldState]);
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
        if (coroutineQueue.Count > 0)
        {
            // TODO: How to handle thrown exceptions from action?
            Func<IEnumerator> action = coroutineQueue.Dequeue();
            processingCoroutine = true;
            yield return StartCoroutine(action());
            processingCoroutine = false;
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

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        foreach (GameState state in Enum.GetValues(typeof(GameState)))
        {
            onStateChangeFromQueues[state] = new Queue<Action>();
            onStateChangeToQueues[state] = new Queue<Action>();
            onStateChangeFromCoroutineQueues[state] = new Queue<Func<IEnumerator>>();
            onStateChangeToCoroutineQueues[state] = new Queue<Func<IEnumerator>>();
        }

        foreach (TriggeredEvent trigger in Enum.GetValues(typeof(TriggeredEvent)))
        {
            onTriggeredEventCoroutineQueues[trigger] = new Queue<Func<IEnumerator>>();
            onTriggeredEventActionQueues[trigger] = new Queue<Action>();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (!processingCoroutine && coroutineQueue.Count > 0)
        {
            StartCoroutine(InvokeNextQueuedCoroutine());
        }
	}

    public float GetMouseDownSpeedMultiplier()
    {
        return Input.GetMouseButton(0) ? this.MousedownSpeedMultiplier : 1.0f;
    }
}

public enum TriggeredEvent
{
    EntityTurnDone,
    CardDrawDone,
}

public enum GameState
{
    Neutral,
    DrawingCard,
    AwaitingCommand,
    CharacterMoving,
    EnemyTurn,
    AwaitingSelection,
}