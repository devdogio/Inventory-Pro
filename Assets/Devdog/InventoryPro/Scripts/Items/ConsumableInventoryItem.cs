using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// Used to represent items that can be used by the player, and once used reduce 1 in stack size. This includes potions, food, scrolls, etc.
    /// </summary>
    public partial class ConsumableInventoryItem : InventoryItemBase
    {
        /// <summary>
        /// When the item is used, play this sound.
        /// </summary>
        public General.AudioClipInfo audioClipWhenUsed;


        public override LinkedList<ItemInfoRow[]> GetInfo()
        {
            var basic = base.GetInfo();
            //basic.AddAfter(basic.First, new InfoBoxUI.Row[]{
            //    new InfoBoxUI.Row("Restore health", restoreHealth.ToString(), Color.green, Color.green),
            //    new InfoBoxUI.Row("Restore mana", restoreMana.ToString(), Color.green, Color.green)
            //});


            return basic;
        }

        public override void NotifyItemUsed(uint amount, bool alsoNotifyCollection)
        {
            base.NotifyItemUsed(amount, alsoNotifyCollection);

            PlayerManager.instance.currentPlayer.inventoryPlayer.stats.SetAll(stats);
        }

        public override int Use()
        {
            int used = base.Use();
            if(used < 0)
                return used;

            if (currentStackSize <= 0)
                return -2;

            // Do something with item
            currentStackSize--; // Remove 1
            
            NotifyItemUsed(1, true);
            AudioManager.AudioPlayOneShot(audioClipWhenUsed);

            return 1; // 1 item used
        }
    }
}