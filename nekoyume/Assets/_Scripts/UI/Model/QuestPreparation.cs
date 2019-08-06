using System;
using Assets.SimpleLocalization;
using Nekoyume.Game.Item;
using Nekoyume.UI.Module;
using UniRx;

namespace Nekoyume.UI.Model
{
    public class QuestPreparation : IDisposable
    {
        private const string DimmedType = nameof(ItemBase.ItemType.Material);

        public readonly ReactiveProperty<Inventory> inventory = new ReactiveProperty<Inventory>();
        public readonly ReactiveProperty<ItemInfo> itemInfo = new ReactiveProperty<ItemInfo>();

        public QuestPreparation(Game.Item.Inventory inventory)
        {
            this.inventory.Value = new Inventory(inventory);
            this.inventory.Value.dimmedFunc.Value = DimmedFunc;
            itemInfo.Value = new ItemInfo();
            itemInfo.Value.buttonText.Value = LocalizationManager.Localize("UI_EQUIP");
            itemInfo.Value.buttonEnabledFunc.Value = null;
            
            this.inventory.Value.selectedItemView.Subscribe(SubscribeInventorySelectedItem);
        }
        
        public void Dispose()
        {
            inventory.DisposeAll();
            itemInfo.DisposeAll();
        }

        private bool DimmedFunc(InventoryItem inventoryItem)
        {
            return inventoryItem.item.Value.Data.cls == DimmedType;
        }
        
        private void SubscribeInventorySelectedItem(InventoryItemView view)
        {
            if (view is null)
            {
                itemInfo.Value.item.Value = null;
                
                return;
            }
            
            itemInfo.Value.item.Value = view.Model;
        }
    }
}
