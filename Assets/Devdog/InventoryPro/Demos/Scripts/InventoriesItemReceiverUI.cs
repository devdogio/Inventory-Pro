using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.Demo
{
    public class InventoriesItemReceiverUI : MonoBehaviour
    {
        private struct SpriteRow
        {
            public Sprite icon;
            public float amount;
        }

        public ItemCollectionSlotUI wrapperPrefab;
        public AnimationClip slideAnimation;

        public float offsetTimerSeconds = 0.2f;

        private ComponentPool<ItemCollectionSlotUI> pool { get; set; }
        private Queue<SpriteRow> queue { get; set; }
        private WaitForSeconds destroyTimer { get; set; }
        private WaitForSeconds offsetTimer { get; set; }


        public IEnumerator Start()
        {
            pool = new ComponentPool<ItemCollectionSlotUI>(wrapperPrefab, 8);
            queue = new Queue<SpriteRow>(8);
            destroyTimer = new WaitForSeconds(slideAnimation.length - 0.025f);
            offsetTimer = new WaitForSeconds(offsetTimerSeconds);

            foreach (var inv in InventoryManager.GetLootToCollections())
            {
                inv.OnAddedItem += (items, amount, cameFromCollection) =>
                {
                    if (cameFromCollection == false)
                    {
                        queue.Enqueue(new SpriteRow() { icon = items.FirstOrDefault().icon, amount = amount});
                    }
                };

                inv.OnCurrencyChanged += (before, after) =>
                {
                    if (after.amount > before)
                    {
                        queue.Enqueue(new SpriteRow() { icon = after.currency.icon, amount = before - after.amount });
                    }
                };
            }

            while (true)
            {
                if (queue.Count > 0)
                {
                    ShowItem(queue.Peek().icon, queue.Peek().amount);
                    queue.Dequeue(); // Remove it
                }

                yield return offsetTimer;
            }
        }

        public void ShowItem(Sprite icon, float amount)
        {
            if (icon != null)
            {
                var inst = pool.Get();

                inst.transform.SetParent(transform);
                inst.transform.localPosition = Vector3.zero;
                inst.transform.SetSiblingIndex(0);

                inst.icon.sprite = icon;
                inst.amountText.text = amount.ToString();
                
                // No repaint, manually handling items
                inst.GetComponent<Animator>().Play(slideAnimation.name);
                StartCoroutine(DestroyItem(inst));
            }
        }

        private IEnumerator DestroyItem(ItemCollectionSlotUI inst)
        {
            yield return destroyTimer;

            pool.Destroy(inst);            
        }
    }   
}