
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TileAI))]
public class BehaviorController : MonoBehaviour
{
    private StrategyChain _chain;
    private TileAI _subject;

    private void Start()
    {
        _subject = GetComponent<TileAI>();
        Debug.Assert(_subject != null, "No TileAI Component along BehaviorController!");
        Debug.Assert(_subject.Behavior != null, "BehaviorList was not set on TileAI data!");
        _chain = new StrategyChain(this, _subject.Behavior);
    }

    public IEnumerator DoStrategy(GameContext context)
    {
        var stats = _subject.GetModifiedStats();
        while (stats.FreeMoves > 0 || stats.FullActions > 0)
        {
            var total = stats.FreeMoves + stats.FullActions;
            var strat = _chain.GetCurrentStrategy(_subject, context);
            yield return strat.Execute(_subject, context);
            stats = _subject.GetModifiedStats();
            if (total == stats.FreeMoves + stats.FullActions)
            {
                // Safety closure to ensure faulty strategies don't loop forever
                Debug.Assert(false, "Number of moves did not change! Make sure all actions cost moves.");
                break;
            }
        }
    }
}

/// <summary>
/// A linked list of StrategyChainNodes which puts behaviors in order in which
/// they appear in a BehaviorList.
/// </summary>
public class StrategyChain
{
    private readonly StrategyChainNode _baseNode;
    private readonly StrategyChainNode _finalNode;
    private readonly IdleStrategy _idleStrat;
    private StrategyChainNode _currentNode;

    public ActorStrategy GetCurrentStrategy(TileAI subject, GameContext context)
    {
        var prevNode = _currentNode;
        if (_currentNode.Strategy.ShouldAbandon(subject, context) || _currentNode == _finalNode)
        {
            // Activity no longer valid, or at final node; start deciding from start again.
            _currentNode = _baseNode;
        }
                
        while (_currentNode != null)
        {
            if (_currentNode.Strategy.CanTakeStrategy(subject) && _currentNode.Strategy.Decide(subject, context))
            {
                break;
            }
                        
            _currentNode = _currentNode.Next;
        }

        if (_currentNode == null)
        {
            Debug.Assert(false, "There should be no null nodes! Should always end with IdleStrat");
            _currentNode = _baseNode;
            return _idleStrat;
        }

        if (prevNode != null && prevNode != _currentNode)
        {
            // Handle transitions to new events
            prevNode.Strategy.ExitStrategy(subject);
            _currentNode.Strategy.EnterStrategy(subject);
        }

        return _currentNode.Strategy;
    }

    public StrategyChain(BehaviorController controller, BehaviorList list)
    {
        _idleStrat = ScriptableObject.CreateInstance<IdleStrategy>();
        
        Debug.Assert(list.Strategies.Length != 0, "Empty BehaviorList used!");
        _baseNode = new StrategyChainNode(Object.Instantiate(list.Strategies[0]));
        var current = _baseNode;
        for (int i = 1; i < list.Strategies.Length; i++)
        {
            current.Next = new StrategyChainNode(Object.Instantiate(list.Strategies[i]));
            current = current.Next;
        }

        _finalNode = new StrategyChainNode(_idleStrat);
        current.Next = _finalNode;
        _currentNode = _baseNode;
    }

    private class StrategyChainNode
    {
        public ActorStrategy Strategy { get; }
        public StrategyChainNode Next;

        public StrategyChainNode(ActorStrategy strategy)
        {
            Strategy = strategy;
        }
    }
}

