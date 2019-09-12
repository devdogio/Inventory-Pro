using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Collection UI/Slot UI Reference Sum")]
    public partial class ItemCollectionSlotUIReferenceSum : ItemCollectionSlotUIKeyTrigger
    {
        protected override void Awake()
        {
            base.Awake();
            this.useCustomUpdate = false;
        }

        public override void Repaint()
        {
            base.Repaint();

            if (item != null)
            {
                uint count = InventoryManager.GetItemCount(item.ID, false);
                amountText.text = count.ToString();
                
                if (count == 0)
                    icon.material = InventorySettingsManager.instance.settings.iconDepletedMaterial;
                else
                    icon.material = InventorySettingsManager.instance.settings.iconDefaultMaterial;
            }
            else
            {
                amountText.text = string.Empty;
                icon.material = InventorySettingsManager.instance.settings.iconDefaultMaterial;
            }
        }

        public override void TriggerUse()
        {
            if (item == null)
                return;

            if (itemCollection.canUseFromCollection == false)
                return;

            var found = InventoryManager.Find(item.ID, false);
            if (found != null)
            {
                int used = found.Use();
                if (used >= 0)
                    found.itemCollection[found.index].Repaint();

                if (itemCollection.useReferences)
                    itemCollection.NotifyReferenceUsed(found, found.ID, index, 1);
            }
        }
    }
}