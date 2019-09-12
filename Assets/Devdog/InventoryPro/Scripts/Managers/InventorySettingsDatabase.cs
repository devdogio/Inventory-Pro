using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.InventoryPro
{
    [CreateAssetMenu(menuName = InventoryPro.CreateAssetMenuPath + "Inventory Settings Database")]
    public class InventorySettingsDatabase : ScriptableObject
    {
        /// <summary>
        /// The default material used when a set of items is higher than 0
        /// </summary>
        [Category("UI")]
        public Material iconDefaultMaterial;

        /// <summary>
        /// The material used when a stack of references is 0 in size. Aka, when the stack is depleted.
        /// </summary>
        public Material iconDepletedMaterial;

        /// <summary>
        /// Extra padding for when the user releases the mouse.
        /// Example: When the user clicks an item and keeps the mouse button down, and moves the cursor outside of the button.
        /// The padding allows the user to release the button further outside of the button, and still trigger it.
        /// </summary>
        public Vector2 onPointerUpInsidePadding;

        /// <summary>
        /// The default UI Item.
        /// </summary>
        [Required]
        public GameObject itemButtonPrefab;

        [Required]
        public CollectionSorterBase collectionSorter;







        [Category("Layers")]
        [Range(0, 31)]
        public int localPlayerLayer = 24;

        /// <summary>
        /// The layer used to equip items
        /// </summary>
        [Range(0, 31)]
        public int equipmentLayer = 25;

        /// <summary>
        /// The layer used when dropping, and when the item is in the world.
        /// </summary>
        [Range(0, 31)]
        public int itemWorldLayer = 26;






        /// <summary>
        /// Use the context menu or not?
        /// </summary>
        [Category("Context menu")]
        public bool useContextMenu;
        







        /// <summary>
        /// Do you want to show a confirmation dialog, when an item is being dropped?
        /// </summary>
        [Category("Dialogs")]
        [Header("Confirmation dialog")]
        public bool showConfirmationDialogWhenDroppingItem = true;
        
        /// <summary>
        /// If true a dialog is displayed, if false the stack will be split in half.
        /// </summary>
        [Header("Unstack dialog")]
        public bool useUnstackDialog = true;


        /// <summary>
        /// The distance items can be used, and windows should be auto closed.
        /// </summary>
        [Category("Pickup, usage & drop")]
        [Required]
        public ItemDropHandlerBase itemDropHandler;

        [Header("Cursor pickup & usage")]
        public CursorIcon pickupCursor;
        public CursorIcon useCursor;

        ///// <summary>
        ///// When the item is clicked, should it trigger?
        ///// </summary>
        [Header("Behavior")]
        [Tooltip("When the item is clicked, should it trigger?")]
        public bool itemTriggerMouseClick = true;

        ///// <summary>
        ///// The key code used when trying to use an item (loot an item)
        ///// </summary>
        [Tooltip("The key code used when trying to use an item (loot an item).")]
        public KeyCode itemTriggerUseKeyCode = KeyCode.None;

        /// <summary>
        /// Always trigger gold pickup on collision, even when itemTriggerOnPlayerCollision is off.
        /// </summary>
        [Tooltip("Always trigger gold pickup on collision, even when itemTriggerOnPlayerCollision is off.")]
        public bool alwaysTriggerGoldItemPickupOnPlayerCollision = false;

        /// <summary>
        /// Trigger the item when the player collides with it.
        /// </summary>
        [Tooltip("Trigger the item when the player collides with it.")]
        public bool itemTriggerOnPlayerCollision = false;


        public Sprite itemTriggerPickupSprite;




        /// <summary>
        /// Unstack when the user clicks the item + has all the required keys down.
        /// </summary>
        [Category("Input")]
        [Header("Unstack actions")]
        public bool useUnstackClick = true;

        /// <summary>
        /// Unstack when the user drags the item to a new slot while holding the required keys.
        /// </summary>
        public bool useUnstackDrag = true;

        /// <summary>
        /// The keys required to unstack
        /// </summary>
        [Header("Action inputs")]
        public InventoryActionInput unstackKeys = new InventoryActionInput(PointerEventData.InputButton.Right, InventoryActionInput.EventType.OnPointerUp, KeyCode.LeftShift);

        /// <summary>
        /// The keys used to "use" an item.
        /// </summary>
        public InventoryActionInput useItemKeys = new InventoryActionInput(PointerEventData.InputButton.Right, InventoryActionInput.EventType.OnPointerUp, KeyCode.None);

        /// <summary>
        /// Trigger the context menu using, the following button
        /// </summary>
        public InventoryActionInput triggerContextMenuKeys = new InventoryActionInput(PointerEventData.InputButton.Left, InventoryActionInput.EventType.OnPointerUp, KeyCode.None);

        [Header("Mobile")]
        public float mobileLongPressTime = 0.3f;

        /// <summary>
        /// How long 2 taps can be apart from one another to trigger the double tap event.
        /// </summary>
        public float mobileDoubleTapTime = 0.4f;
    }
}
