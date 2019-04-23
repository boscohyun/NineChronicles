using Nekoyume.Game.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI
{
    public class EquipSlot : MonoBehaviour
    {
        public GameObject button;
        public Image icon;
        public ItemBase item;
        public ItemBase.ItemType type;



        public void Equip(SelectedItem selected)
        {
            icon.overrideSprite = selected.icon.sprite;
            icon.gameObject.SetActive(true);
            icon.SetNativeSize();
            item = selected.item;
            if (button != null)
            {
                button.gameObject.SetActive(true);
            }
        }
        public void Unequip()
        {
            icon.gameObject.SetActive(false);
            item = null;
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }

        public void Set(Equipment equipment)
        {
            var sprite = ItemBase.GetSprite(equipment);
            icon.overrideSprite = sprite;
            icon.gameObject.SetActive(true);
            icon.SetNativeSize();
            item = equipment;
            if (button != null)
            {
                button.gameObject.SetActive(true);
            }
        }
    }
}
