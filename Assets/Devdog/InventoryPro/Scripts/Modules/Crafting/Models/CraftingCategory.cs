using UnityEngine;
using System;
using System.Collections.Generic;
using Devdog.General;
using UnityEngine.UI;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class CraftingCategory : ScriptableObject
    {
        [HideInInspector]
        public int ID;

        [Required]
        public new string name;
        public string description;
        public Sprite icon;

        /// <summary>
        /// Also scan through the bank for items to use when crafting the item.
        /// </summary>
        public bool alsoScanBankForRequiredItems = false;

        public uint rows = 3;
        public uint cols = 3;

        public CraftingBlueprint[] blueprints = new CraftingBlueprint[0];

        public AudioClipInfo successAudioClip = new AudioClipInfo();
        public AudioClipInfo craftingAudioClip = new AudioClipInfo() { loop = true };
        public AudioClipInfo canceledAudioClip = new AudioClipInfo();
        public AudioClipInfo failedAudioClip = new AudioClipInfo();

        public override string ToString()
        {
            return name;
        }
    }
}