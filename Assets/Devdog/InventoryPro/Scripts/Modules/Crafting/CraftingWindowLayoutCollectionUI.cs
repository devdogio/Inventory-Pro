using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    public partial class CraftingWindowLayoutCollectionUI : ItemCollectionBase
    {
        //[Header("Behavior")] // Moved to custom editor

        [SerializeField]
        private uint _initialCollectionSize = 9;
        public override uint initialCollectionSize
        {
            get
            {
                return _initialCollectionSize;
            }
        }

        protected override void Awake()
        {
            base.Awake();

//            this._craftingWindow = GetComponent<CraftingWindowLayoutUI>();
//            _canDragInCollectionDefault = canDragInCollection;
        }
    }
}