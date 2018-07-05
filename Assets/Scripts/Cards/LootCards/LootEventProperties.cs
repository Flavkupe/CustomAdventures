using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class LootEventProperties
{
    [Tooltip("Filter for types of loot filtered during the loot event. If empty, will produce all types.")]
    public LootCardType[] LootTypes;

    [Tooltip("How many treasures this loot event will produce")]
    public int NumTreasures = 1;

    public LootCardFilter CreateLootCardFilter()
    {
        var filter = LootTypes != null && LootTypes.Length > 0 ? new LootCardFilter() : null;
        if (filter != null)
        {
            LootTypes.ToList().ForEach(a => filter.PossibleTypes.Add(a));
        }

        return filter;
    }
}

public interface IProducesLootEvent
{
    event EventHandler<LootEventProperties> LootEventRequested;
}

public class LootCardFilter
{
    public HashSet<LootCardType> PossibleTypes = new HashSet<LootCardType>();
}