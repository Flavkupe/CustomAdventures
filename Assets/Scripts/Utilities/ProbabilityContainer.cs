using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProbabilityContainer<TObjectType> where TObjectType : class
{
    private int _runningTotal = 0;

    private WeightedPair _lastAdded = null;

    private readonly List<WeightedPair> _list = new List<WeightedPair>();

    private bool _needsSort = true;

    private class WeightedPair : IComparable<WeightedPair>
    {
        public TObjectType Item { get; private set; }

        public int Weight { get; private set; }

        public int Rank { get; set; }

        public WeightedPair(TObjectType item, int weight)
        {
            Item = item;
            Weight = weight;
        }

        public int CompareTo(WeightedPair other)
        {
            return Rank.CompareTo(other.Rank);
        }
    }

    public void AddItem(TObjectType item, int weight)
    {
        _runningTotal += weight;
        var pair = new WeightedPair(item, weight)
        {
            Rank = _lastAdded == null ? weight : _lastAdded.Rank + weight
        };

        _list.Add(pair);
        _lastAdded = pair;
        _needsSort = true;
    }

    public TObjectType GetRandom()
    {
        if (_list.Count <= 1)
        {
            return _lastAdded == null ? null : _lastAdded.Item;
        }

        if (_needsSort)
        {
            _list.Sort();
            _needsSort = false;
        }

        var random = UnityEngine.Random.Range(0, _runningTotal + 1);
        foreach (var pair in _list)
        {
            if (random <= pair.Rank)
            {
                return pair.Item;
            }
        }

        Debug.Assert(false, "Shouldn't hit this; check range function!");
        return _list.First().Item;
    }
}
