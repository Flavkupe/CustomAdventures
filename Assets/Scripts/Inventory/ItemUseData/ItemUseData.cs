using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ItemUseData : ScriptableObject
{
    public AudioClip[] ItemUsedSounds;

    public abstract bool ItemUsed(InventoryItem item, ItemUseContext context);
}

