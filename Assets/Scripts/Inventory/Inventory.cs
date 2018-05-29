using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Inventory<T> where T : class, IStackable
{
    public Inventory() { }

    public Inventory(IEnumerable<T> items)
    {
        _items.AddRange(items);
    }

    private List<T> _items = new List<T>();

    public bool UnlimittedItems = false;
    public int MaxItems = 6;

    public bool Contains(T item)
    {
        return _items.Contains(item);
    }

    public bool TryRemoveItem(T item)
    {
        if (Contains(item))
        {
            _items.Remove(item);
            return true;
        }

        return false;
    }

    public bool TryAddItem(T item)
    {
        bool madeChanges = false;
        foreach (T current in _items)
        {
            if (current.ItemCanStack(item))
            {
                current.StackItems(item);
                madeChanges = true;
                if (item.StackSize == 0)
                {
                    break;
                }
            }
        }

        if (item.StackSize > 0 && (UnlimittedItems || _items.Count < MaxItems))
        {
            madeChanges = true;
            _items.Add(item);
        }

        return madeChanges;
    }

    public int Count { get { return _items.Count; } }

    public T GetFirstOrDefault(Func<T, bool> condition)
    {
        return _items.FirstOrDefault(condition);
    }

    public T Get(int index)
    {        
        if (index < 0 || index >= _items.Count)
        {
            Debug.Assert(false, "Trying to access indexisting item");
            return null;
        }

        return _items[index];
    }
}

