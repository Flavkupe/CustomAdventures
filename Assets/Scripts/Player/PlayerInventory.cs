using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

[Serializable]
public class PlayerInventory
{
    public InventoryItem<WeaponCardData> EquippedWeapon { get { return this.GetInventoryItem<WeaponCardData>(InventoryItemType.Weapon); } }
    
    public int MaxItems
    {
        get { return InventoryItems.MaxItems; }
        set { InventoryItems.MaxItems = value; }
    } 

    public Inventory<InventoryItem> InventoryItems = new Inventory<InventoryItem>();
    public Dictionary<InventoryItemType, InventoryItem> EquipmentItems = new Dictionary<InventoryItemType, InventoryItem>();

    public PlayerInventory()
    {
        EnumUtils.DoForeachEnumValue<InventoryItemType>(item =>
        {
            if (item != InventoryItemType.Misc)
            {
                EquipmentItems[item] = null;
            }
        });
    }

    public bool IsSlotOccupied(InventoryItemType type)
    {
        return GetEquipmentItem(type) != null;
    }

    public InventoryItem<TDataType> GetInventoryItem<TDataType>(InventoryItemType type) where TDataType : ItemCardData
    {
        var item = EquipmentItems[type];
        Debug.Assert(item == null || item is InventoryItem<TDataType>, "Invalid ItemData cast!");
        return item as InventoryItem<TDataType>;
    }

    /// <summary>
    /// Changes the equipment without making any other changes
    /// </summary>
    public void UnequipInventoryItemDirectly(InventoryItem item)
    {
        EquipmentItems[item.Type] = null;
    }

    /// <summary>
    /// Changes the equipment without making any other changes
    /// </summary>
    public void EquipInventoryItemDirectly(InventoryItem item)
    {
        EquipmentItems[item.Type] = item;
    }

    /// <summary>
    /// Adds items to inventory without making any other changes.
    /// Returns false if there is no room.
    /// </summary>
    public bool AddInventoryItemDirectly(InventoryItem item)
    {
        return InventoryItems.TryAddItem(item);
    }

    public InventoryItem GetEquipmentItem(InventoryItemType type)
    {
        return EquipmentItems.ContainsKey(type) ? EquipmentItems[type] : null;
    }

    public List<InventoryItem> GetDefensiveItems()
    {
        return EquipmentItems.Values.Where(a => a != null && a.DefenseValue > 0).ToList();
    }

    public void Equip(InventoryItem item)
    {
        EquipInventoryItemDirectly(item);
        item.ItemEquipped();
        Game.UI.UpdateInventory();
        Game.UI.UpdateEntityPanels();
    }

    public bool Unequip(InventoryItemType item)
    {
        if (IsSlotOccupied(item))
        {
            return Unequip(GetEquipmentItem(item));
        }

        return false;
    }

    public bool Unequip(InventoryItem item)
    {
        if (TryMoveToInventory(item, false))
        {
            UnequipInventoryItemDirectly(item);
            item.ItemUnequipped();
            Game.UI.UpdateInventory();
            Game.UI.UpdateEntityPanels();
            return true;
        }

        return false;
    }

    public void ClearEquipmentItem(InventoryItem item, bool updateUI = true)
    {
        if (item == null)
        {
            return;
        }

        if (EquipmentItems.ContainsKey(item.Type) && EquipmentItems[item.Type] == item)
        {
            UnequipInventoryItemDirectly(item);
            item.ItemUnequipped();
        }

        if (updateUI)
        {
            Game.UI.UpdateInventory();
            Game.UI.UpdateEntityPanels();
        }
    }

    public void ClearInventoryItem(InventoryItem item, bool updateUI = true)
    {
        if (item == null)
        {
            return;
        }

        if (!InventoryItems.TryRemoveItem(item))
        {
            ClearEquipmentItem(item, false);
        }

        if (updateUI)
        {
            Game.UI.UpdateInventory();
            Game.UI.UpdateEntityPanels();
        }
    }

    public bool DiscardEquipment(InventoryItem item)
    {
        if (item == null)
        {
            return false;
        }

        if (!IsSlotOccupied(item.Type))
        {
            Debug.Assert(false, "Attempting to discard equipment that is not equiped.");
            return false;
        }

        ClearEquipmentItem(item, false);
        item.PlayItemDroppedSound();
        DropOnGround(item);
        Game.UI.UpdateInventory();
        return true;
    }

    public bool DiscardItem(InventoryItem item, bool fromInventory = true)
    {
        if (item == null)
        {
            return false;
        }

        if (fromInventory)
        {
            InventoryItems.TryRemoveItem(item);
        }

        item.PlayItemDroppedSound();
        DropOnGround(item);
        Game.UI.UpdateInventory();
        return true;
    }

    private static void DropOnGround(InventoryItem item)
    {
        var grid = Game.Dungeon.Grid;
        var playerX = Game.Player.XCoord;
        var playerY = Game.Player.YCoord;
        grid.PutPassableItem(playerX, playerY, item.AsPassableTileItem(), true);        
    }

    public bool TryLootItemFromGround(InventoryItem item)
    {
        if (item == null)
        {
            return false;
        }

        if (TryMoveToInventory(item, false))
        {
            var grid = Game.Dungeon.Grid;
            if (grid.TryClearTileItem(Game.Player.XCoord, Game.Player.YCoord, item))
            {
                Game.UI.UpdateInventory();
            }
        }

        return false;
    }

    public bool TryMoveToInventory(InventoryItem item, bool updateUI, bool autoEquip = true)
    {
        if (item == null)
        {
            return false;
        }

        bool madeChanges = false;
        if (autoEquip && item.IsEquipment && !IsSlotOccupied(item.Type))
        {
            Equip(item);
            madeChanges = true;
        }
        else if (InventoryItems.TryAddItem(item))
        {
            item.PlayItemLootedSound();
            madeChanges = true;
        }

        if (madeChanges && updateUI)
        {
            Game.UI.UpdateInventory();
        }

        return madeChanges;
    }
}