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

    // TODO
    public InventoryItem<WeaponCardData> EquippedArmor;
    public InventoryItem<WeaponCardData> EquippedAccessory;

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

    public void UnequipInventoryItem(InventoryItemType type)
    {
        EquipmentItems[type] = null;
    }

    public void EquipInventoryItem(InventoryItem item)
    {
        EquipmentItems[item.Type] = item;
    }

    public InventoryItem GetEquipmentItem(InventoryItemType type)
    {
        return EquipmentItems.ContainsKey(type) ? EquipmentItems[type] : null;
    }

    public void Equip(InventoryItem item)
    {
        InventoryItem current = null;
        if (IsSlotOccupied(item.Type))
        {
            current = GetEquipmentItem(item.Type);            
        }
        
        EquipInventoryItem(item);
        InventoryItems.TryRemoveItem(item);

        if (current != null)
        {
            TryMoveToInventory(item, false);
        }

        Game.UI.UpdateInventory();
        Game.UI.UpdateUI();
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
            UnequipInventoryItem(item.Type);
            Game.UI.UpdateInventory();
            Game.UI.UpdateUI();
            return true;
        }

        return false;
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

        var grid = Game.Dungeon.Grid;
        var playerX = Game.Player.XCoord;
        var playerY = Game.Player.YCoord;
        grid.PutPassableItem(playerX, playerY, item.AsPassableTileItem(), true);
        Game.UI.UpdateInventory();
        return true;
    }

    public bool TryLootItem(InventoryItem item)
    {
        if (item == null)
        {
            return false;
        }

        if (TryMoveToInventory(item, false))
        {
            var grid = Game.Dungeon.Grid;
            var playerX = Game.Player.XCoord;
            var playerY = Game.Player.YCoord;
            var tileItems = grid.Get(playerX, playerY).TileItems;
            var tileItem = tileItems.GetFirstOrDefault(a => a.Item == item);
            if (tileItem != null)
            {
                grid.ClearPassableTileItem(tileItem);
                Game.UI.UpdateInventory();
                return true;
            }
            else
            {
                Debug.Assert(false, "Trying to loot item not in tile!");
            }
        }

        return false;
    }

    public bool TryMoveToInventory(InventoryItem item, bool updateUI)
    {
        if (item == null)
        {
            return false;
        }

        bool madeChanges = false;
        if (item.IsEquipment && !IsSlotOccupied(item.Type))
        {
            Equip(item);
            madeChanges = true;
        }
        else
        {
            madeChanges = InventoryItems.TryAddItem(item);
        }

        if (madeChanges && updateUI)
        {
            Game.UI.UpdateInventory();
        }

        return madeChanges;
    }    
}