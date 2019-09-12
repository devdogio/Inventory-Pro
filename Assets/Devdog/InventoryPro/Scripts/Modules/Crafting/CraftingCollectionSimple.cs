using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.InventoryPro;
using UnityEngine;
using Devdog.InventoryPro.UI;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    public partial class CraftingCollectionSimple : ItemCollectionBase
    {
        [SerializeField]
        private uint _initialCollectionSize;
        public override uint initialCollectionSize
        {
            get { return _initialCollectionSize; }
        }


        public override bool OverrideUseMethod(InventoryItemBase item)
        {
//            InventoryManager.AddItemAndRemove(item);
            return true;
        }
    }
}
