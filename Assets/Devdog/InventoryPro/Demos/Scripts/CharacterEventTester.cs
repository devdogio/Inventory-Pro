using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;

namespace Devdog.InventoryPro.Demo
{
    public partial class CharacterEventTester : MonoBehaviour
    {

        [Required]
        public CharacterUI character;
        
        public void Awake()
        {
            character.OnAddedItem += (items, amount, cameFromCollection) =>
            {
                Debug.Log("Character collection : added item");
            };
            character.OnRemovedItem += (item, id, slot, amount) =>
            {
                Debug.Log("Character collection : removed item");
            };
            character.OnSwappedItems += (collection, slot, toCollection, toSlot) =>
            {
                Debug.Log("Character collection : swapped items");
            };
            character.OnUsedItem += (item, id, slot, amount) =>
            {
                Debug.Log("Character collection : use item from collection");
            };
        }
    }
}
