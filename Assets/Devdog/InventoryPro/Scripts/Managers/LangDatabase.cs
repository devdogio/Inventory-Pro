using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine;


namespace Devdog.InventoryPro
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LanguageDatabase.asset", menuName = InventoryPro.ProductName + "/Language Database")]
    public partial class LangDatabase : ScriptableObject
    {
        [Category("Item actions")]
        public InventoryNoticeMessage itemCannotBeDropped = new InventoryNoticeMessage("", "Item {0} cannot be dropped", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage itemCannotBeStored = new InventoryNoticeMessage("", "Item {0} cannot be stored", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage itemCannotBeUsed = new InventoryNoticeMessage("", "Item {0} cannot be used", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage itemCannotBeUsedLevelToLow = new InventoryNoticeMessage("", "Item {0} cannot be used required level is {2}", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage itemCannotBeSold = new InventoryNoticeMessage("", "Item {0} cannot be sold", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage itemCannotBePickedUpToFarAway = new InventoryNoticeMessage("", "Item {0} is too far away to pick up", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage itemIsInCooldown = new InventoryNoticeMessage("", "Item {0} is in cooldown {2:0.00} more seconds", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage itemCannotBeUsedToLowStat = new InventoryNoticeMessage("", "Item {0} cannot be used {2} is to low", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage itemCannotBeUsedStatNotValid = new InventoryNoticeMessage("", "Item {0} cannot be used {2} is to high or to low", NoticeDuration.Medium, Color.white);

        //public InventoryNoticeMessage cannotDropItem;

        [Category("Collections")]
        public InventoryNoticeMessage collectionDoesNotAllowType = new InventoryNoticeMessage("", "Does not allow type", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage collectionFull = new InventoryNoticeMessage("", "{2} is full", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage collectionExceedingMaxWeight = new InventoryNoticeMessage("", "Item {0} is to heavy to pick up", NoticeDuration.Medium, Color.white);
        //public InventoryNoticeMessage collection;
        //public InventoryNoticeMessage collectionDoesNotAllowType;

        [Category("Triggers")]
        public InventoryNoticeMessage triggerCantBeUsedMissingItems = new InventoryNoticeMessage("", "{0} can't be used. Missing {1}", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage triggerCantBeUsedFailedStatRequirements = new InventoryNoticeMessage("", "{0} can't be used. {1} is to high or to low", NoticeDuration.Medium, Color.white);


        [Category("User actions")]
        public InventoryNoticeMessage userNotEnoughGold = new InventoryNoticeMessage("", "Not enough gold", NoticeDuration.Medium, Color.white);


        [Category("Crafting")]
        public InventoryNoticeMessage craftedItem = new InventoryNoticeMessage("", "Successfully crafted {0}", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage craftingFailed = new InventoryNoticeMessage("", "Crafting item {0} failed", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage craftingCanceled = new InventoryNoticeMessage("", "Crafting item {0} canceled", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage craftingDontHaveRequiredItems = new InventoryNoticeMessage("", "You don't have the required items to craft {2}", NoticeDuration.Long, Color.white);
        public InventoryNoticeMessage craftingCannotStatNotValid = new InventoryNoticeMessage("", "Item {0} cannot be crafted {2} is to high or to low", NoticeDuration.Medium, Color.white);


        [Category("Vendor")]
        public InventoryNoticeMessage vendorCannotSellItem = new InventoryNoticeMessage("", "Cannot sell item {0} to this vendor.", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage vendorSoldItemToVendor = new InventoryNoticeMessage("", "Sold {2}x {0} to vendor {3} for {4}.", NoticeDuration.Medium, Color.white);
        public InventoryNoticeMessage vendorBoughtItemFromVendor = new InventoryNoticeMessage("", "Bought {2}x {0} from vendor {3} for {4}.", NoticeDuration.Medium, Color.white);


        [Category("Dialogs")]
        public InventoryMessage confirmationDialogDrop = new InventoryMessage("Are you sure?", "Are you sure you want to drop {0}?");
        public InventoryMessage unstackDialog = new InventoryMessage("Unstack item {0}", "How many do you want to unstack?");
    }
}
