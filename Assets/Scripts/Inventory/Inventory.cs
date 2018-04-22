using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Inventory
{
    private List<InventoryItem> _items = new List<InventoryItem>();

    public int MaxItems = 6;

    public bool TryRemoveItem(InventoryItem item)
    {
        if (_items.Contains(item))
        {
            _items.Remove(item);
            return true;
        }

        return false;
    }

    public bool TryAddItem(InventoryItem item)
    {
        bool madeChanges = false;
        foreach (InventoryItem current in _items)
        {
            if (current.ItemCanStack(item))
            {
                current.StackItems(item);
                madeChanges = true;
                if (item.CurrentStackSize == 0)
                {
                    break;
                }
            }
        }

        if (item.CurrentStackSize > 0 && _items.Count < MaxItems)
        {
            madeChanges = true;
            _items.Add(item);
        }

        return madeChanges;
    }

    public int Count { get { return _items.Count; } }

    public InventoryItem Get(int index)
    {        
        if (index < 0 || index >= _items.Count)
        {
            Debug.Assert(false, "Trying to access indexisting item");
            return null;
        }

        return _items[index];
    }
}

