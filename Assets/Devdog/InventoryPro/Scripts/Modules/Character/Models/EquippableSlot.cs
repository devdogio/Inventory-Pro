using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    using Devdog.General.ThirdParty.UniLinq;

//    [RequireComponent(typeof(ItemCollectionSlotUI))]
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "UI Helpers/Equippable slot")]
    public partial class EquippableSlot : MonoBehaviour
    {
        /// <summary>
        /// Index of this slot
        /// </summary>
        public uint index
        {
            get
            {
                return slot.index;
            }
        }

        [SerializeField]
        private EquipmentType[] _equipmentTypes = new EquipmentType[0];
        public EquipmentType[] equipmentTypes
        {
            get { return _equipmentTypes; }
            protected set { _equipmentTypes = value; }
        }

        private ICharacterCollection _characterCollection;
        public ICharacterCollection characterCollection
        {
            get
            {
                if (_characterCollection == null)
                {
                    _characterCollection = GetComponentsInParent<ICharacterCollection>(true).FirstOrDefault();
                    Assert.IsNotNull(_characterCollection, "ICharacterCollection couldn't be found in parent. Equippable slot error!");
                }

                return _characterCollection;
            }
        }

        private ItemCollectionSlotUIBase _slot;
        public ItemCollectionSlotUIBase slot
        {
            get
            {
                if (_slot == null)
                    _slot = GetComponent<ItemCollectionSlotUIBase>();

                return _slot;
            }
        }


        protected void Awake()
        {
            
        }

        protected void Start()
        {
            
        }
    }
}