
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AbilityPanel : SingletonObject<AbilityPanel>
{
    private List<AbilityPanelSlot> slots = new List<AbilityPanelSlot>();

    public bool HasEmpty
    {
        get { return slots.Any(a => a.IsEmpty); }
    }

    public void AddAbility(IAbilityCard card)
    {
        if (HasEmpty)
        {
            AbilityPanelSlot slot = slots.FirstOrDefault(a => a.IsEmpty);
            if (slot != null)
            {
                slot.SetAbility(card);
                return;
            }      
        }

        Debug.Assert(false, "Cannot add ability if slot is empty!");
    }

    public void SyncSlotsWithPlayer()
    {
        foreach (var slot in slots)
        {
            slot.Clear();
        }

        int i = 0;        

        if (slots.Count < Game.Player.Abilities.Count)
        {
            Debug.Assert(false, "More abilities than slots!!");
        }

        foreach (var ability in Game.Player.Abilities)
        {
            slots[i].SetAbility(ability);
            i++;
            if (i >= slots.Count)
            {
                return;
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        slots.AddRange(GetComponentsInChildren<AbilityPanelSlot>());
    }
}

