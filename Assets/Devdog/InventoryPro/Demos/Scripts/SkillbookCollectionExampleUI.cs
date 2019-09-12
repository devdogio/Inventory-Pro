using UnityEngine;
using System.Collections;


namespace Devdog.InventoryPro
{
    public class SkillbookCollectionExampleUI : ItemCollectionBase
    {
        [SerializeField]
        private uint _collectionSize = 10;
        public override uint initialCollectionSize
        {
            get { return _collectionSize; }
        }
        
        public override bool SwapOrMerge(uint slot1, ItemCollectionBase handler2, uint slot2, bool repaint = true)
        {
            if (this == handler2)
                return false;

            if (handler2[slot2].item == null)
            {
                return base.SwapOrMerge(slot1, handler2, slot2, repaint);
            }

            return false;
        }
    }
}