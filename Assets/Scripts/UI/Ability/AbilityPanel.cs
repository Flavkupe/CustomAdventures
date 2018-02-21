
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AbilityPanel : SingletonObject<AbilityPanel>
{
    List<AbilityPanelSlot> slots = new List<AbilityPanelSlot>();

    public bool HasEmpty
    {
        get { return this.slots.Any(a => a.IsEmpty); }
    }

    public void AddAbility(IAbilityCard card)
    {
        if (this.HasEmpty)
        {
            AbilityPanelSlot slot = this.slots.FirstOrDefault(a => a.IsEmpty);
            if (slot != null)
            {
                slot.SetAbility(card);
                return;
            }      
        }

        Debug.Assert(false, "Cannot add ability if slot is empty!");
    }

    private void Awake()
    {
        Instance = this;
        this.slots.AddRange(this.GetComponentsInChildren<AbilityPanelSlot>());
    }
}

