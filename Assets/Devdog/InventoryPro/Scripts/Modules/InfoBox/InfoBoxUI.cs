using System;
using UnityEngine;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using UnityEngine.UI;
using Devdog.InventoryPro.UI;
using Devdog.General.UI;

namespace Devdog.InventoryPro
{
    [RequireComponent(typeof(Devdog.General.UI.UIWindow))]
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Windows Other/Infobox")]
    public partial class InfoBoxUI : MonoBehaviour
    {
        public delegate void Repainted(InventoryItemBase item, LinkedList<ItemInfoRow[]> rows);
        public event Repainted OnRepainted;


        /// <summary>
        /// All information will be appended to the container.
        /// </summary>
        [Header("UI")]
        public RectTransform container;

        // Default fields
        public Image uiIcon;
        public Text uiName;
        public Text uiDescription;



        /// <summary>
        /// Show the dialog window when hovering a lootable item.
        /// </summary>
        [Header("UI behavior")]
        public ItemCollectionBase[] collectionsToIgnore = new ItemCollectionBase[0];
        public bool showWhenHoveringLootableObject = true;
        public bool showWhenHoveringSlot = true;
        public bool showWhenClickSlot = true;

        /// <summary>
        /// Should the window be shown / hidden when a new item becomes visible / invisible.
        /// </summary>
        public bool showHideWindow = true;

        /// <summary>
        /// Show the dialog for out of range items?
        /// </summary>
        public bool showOutOfRangeLootables = false;

        /// <summary>
        /// Position the info box at the cursor's position?
        /// </summary>
        public bool positionAtMouse = true;

        /// <summary>
        /// Show the info box for the best trigger ( not the hovering one )
        /// </summary>
        public bool showForBestTrigger = true;

        /// <summary>
        /// Show the info gotten from the item's GetInfo() method?
        /// </summary>
        public bool showItemInfo = true;

        /// <summary>
        /// Show the usage requirements of items?
        /// </summary>
        [Header("Usage requirements")]
        public bool showUsageRequirements = true;

        public bool overrideUsageRequirementString = false;
        public string overrideUsageRequirementStringFormat = "{0}";

        public bool changeUsageRequirementsColors = true;
        public Color usageRequirementToLowColor = Color.red;
        public Color usageRequirementHighEnoughColor = Color.green;

        /// <summary>
        /// When the InfoBoxUI hits the right or left part of the screen it will move to the other side.
        /// </summary>
        [Header("Borders")]
        public bool moveWhenHitBorderHorizontal = true;

        /// <summary>
        /// When the InfoBoxUI hits the top or bottom part of the screen it will move to the other side.
        /// </summary>
        public bool moveWhenHitBorderVertical = true;

        /// <summary>
        /// Used to define extra margin on the corners of the screen.
        /// If the item falls of the screen it will be shown on the other side of the cursor.
        /// </summary>
        public Vector2 borderMargins;

        [Header("UI element prefabs")]
        [Required]
        public GameObject infoBoxCategory;
        public GameObject separatorPrefab;
        [Required]
        public InfoBoxRowUI infoBoxRowPrefab; // 1 item (row) inside the infobox



        private RectTransform rectTransform { get; set; }
        private Vector2 defaultPivot { get; set; }

        protected InventoryItemBase currentItem;
        protected TriggerBase currentTrigger;

        private Devdog.General.UI.UIWindow window { get; set; }
        protected ComponentPool<InfoBoxRowUI> poolRows { get; set; }
        protected GameObjectPool poolSeparators { get; set; }
        protected GameObjectPool poolCategoryBoxes { get; set; }


        private bool isHoveringSlotWithItem
        {
            get
            {
                var a = InventoryUIUtility.currentlyHoveringSlot;
                return a != null && a.item != null;
            }
        }

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            defaultPivot = rectTransform.pivot;
            window = GetComponent<Devdog.General.UI.UIWindow>();

            poolRows = new ComponentPool<InfoBoxRowUI>(infoBoxRowPrefab, 32);
            if(separatorPrefab != null)
                poolSeparators = new GameObjectPool(separatorPrefab, 8);

            poolCategoryBoxes = new GameObjectPool(infoBoxCategory, 8);
        }

        protected virtual void Update()
        {
            if (isHoveringSlotWithItem)
            {
                if (showWhenHoveringSlot)
                {
                    HandleInfoBox(InventoryUIUtility.currentlyHoveringSlot.item);
                }
                else if(showWhenClickSlot)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        HandleInfoBox(InventoryUIUtility.currentlyHoveringSlot.item);
                    }
                }
            }
            else
            {
                if (UIUtility.isHoveringUIElement == false && 
                    TriggerUtility.isMouseOnTrigger)
                {
                    if(showWhenHoveringLootableObject)
                    {
                        HandleItemTrigger(TriggerUtility.mouseOnTrigger);
                    }
                }
                else
                {
                    if (showForBestTrigger &&
                        PlayerManager.instance.currentPlayer != null &&
                        PlayerManager.instance.currentPlayer.triggerHandler != null &&
                        PlayerManager.instance.currentPlayer.triggerHandler.selectedTrigger != null)
                    {
                        HandleItemTrigger(PlayerManager.instance.currentPlayer.triggerHandler.selectedTrigger);
                    }
                    else
                    {
                        HandleInfoBox(null);
                    }
                }
            }
        }

        protected virtual void HandleItemTrigger(TriggerBase trigger)
        {
            if (trigger != null)
            {
                if (trigger.inRange == false && showOutOfRangeLootables == false)
                {
                    return;
                }

                var a = trigger as ItemTrigger;
                if (a != null)
                {
                    currentTrigger = a;
                    HandleInfoBox(a.itemPrefab ?? a.GetComponent<InventoryItemBase>());
                    return;
                }
            }

            if(isHoveringSlotWithItem == false)
                Hide();
        }

        public virtual void HandleInfoBox(InventoryItemBase forItem)
        {
            var a = InventoryUIUtility.currentlyHoveringSlot;
            if (forItem == null || (a != null && collectionsToIgnore.Contains(a.itemCollection)))
            {
                Hide();
                return;
            }

            if (forItem != currentItem)
            {
                currentItem = forItem;
                Repaint(currentItem, currentItem.GetInfo());
            }

            if (showHideWindow)
            {
                window.Show();
            }
        }

        protected virtual void HandleBorders()
        {
            if (InventoryManager.instance.uiRoot.renderMode == RenderMode.WorldSpace)
                return;


            if (moveWhenHitBorderHorizontal)
            {
                // Change the box if its about to fall of the screen
                if (Input.mousePosition.x + rectTransform.sizeDelta.x > Screen.width - borderMargins.x)
                {
                    // Falls of the right
                    rectTransform.pivot = new Vector2(defaultPivot.y, rectTransform.pivot.x); // Swap
                }
                else
                {
                    rectTransform.pivot = new Vector2(defaultPivot.x, rectTransform.pivot.y); // Swap
                }
            }

            if (moveWhenHitBorderVertical)
            {
                if (Input.mousePosition.y - rectTransform.sizeDelta.y < 0.0f - borderMargins.y)
                {
                    // Falls of the bottom
                    rectTransform.pivot = new Vector2(rectTransform.pivot.x, defaultPivot.x); // Swap                
                }
                else
                {
                    rectTransform.pivot = new Vector2(rectTransform.pivot.x, defaultPivot.y); // Swap
                }
            }
        }

        protected virtual void PositionInfoBox()
        {
            if (positionAtMouse)
            {
                InventoryUIUtility.PositionRectTransformAtPosition(rectTransform, Input.mousePosition);
                HandleBorders();
            }
        }

        protected virtual void Hide()
        {
            currentItem = null;
            if (showHideWindow)
                window.Hide();

        }

        protected virtual void LateUpdate()
        {
            if(window.isVisible)
                PositionInfoBox();
        }
        
        /// <summary>
        /// Repaint the infobox with the given data.
        /// </summary>
        /// <param name="item">The item we're going to display</param>
        /// <param name="itemInfo">The rows of data we're displaying</param>
        protected virtual void Repaint(InventoryItemBase item, LinkedList<ItemInfoRow[]> itemInfo)
        {
            poolRows.DestroyAll();
            poolSeparators.DestroyAll();
            poolCategoryBoxes.DestroyAll();

            // The usual stuff
            if (uiIcon != null)
                uiIcon.sprite = item.icon;
            
            if (uiName != null)
            {
                uiName.text = item.name;
                uiName.color = (item.rarity != null) ? item.rarity.color : uiName.color;
            }
            if (uiDescription != null)
                uiDescription.text = item.description;


            if (showItemInfo)
            {
                DrawItemInfo(item, itemInfo);
            }

            // have we got usagerequirements?
            if (showUsageRequirements && item.usageRequirement.Length > 0)
            {
                DrawUsageRequirements(item);
            }

            if (OnRepainted != null)
                OnRepainted(item, itemInfo);
        }

        protected virtual void DrawItemInfo(InventoryItemBase item, LinkedList<ItemInfoRow[]> itemInfo)
        {
            int i = 0;
            foreach (var box in itemInfo)
            {
                i++;

                var boxObj = poolCategoryBoxes.Get();

                int addedRows = 0;
                foreach (var row in box)
                {
                    var rowObj = poolRows.Get();
                    rowObj.transform.SetParent(boxObj.transform);
                    InventoryUtility.ResetTransform(rowObj.transform);

                    rowObj.title.text = row.title;
                    rowObj.title.color = row.titleColor;

                    rowObj.message.text = row.text;
                    rowObj.message.color = row.textColor;

                    addedRows++;
                }

                boxObj.transform.SetParent(container);
                InventoryUtility.ResetTransform(boxObj.transform);

                if (i < itemInfo.Count && addedRows > 0 && separatorPrefab != null)
                {
                    // Add a separator
                    if (separatorPrefab != null)
                    {
                        var separator = poolSeparators.Get();
                        separator.transform.SetParent(container);
                        InventoryUtility.ResetTransform(separator.transform);
                    }
                }
            }
        }

        protected virtual void DrawUsageRequirements(InventoryItemBase item)
        {
            var separator = poolSeparators.Get();
            separator.transform.SetParent(container);
            InventoryUtility.ResetTransform(separator.transform);

            var box = poolCategoryBoxes.Get();

            // loop through all usage requirements...
            foreach (var itemProperty in item.usageRequirement)
            {
                // row title, message...
                var row = poolRows.Get();
                var prop = itemProperty.stat;

                row.transform.SetParent(box.transform);
                InventoryUtility.ResetTransform(row.transform);

                row.title.text = prop.statName;
                row.title.color = prop.color;

                row.message.text = prop.ToString(itemProperty.value,
                    overrideUsageRequirementString ? overrideUsageRequirementStringFormat : prop.valueStringFormat);

                if (changeUsageRequirementsColors)
                {
                    var p = PlayerManager.instance.currentPlayer;
                    if (p != null)
                    {
                        if(itemProperty.CanUse(p.inventoryPlayer))
                        {
                            row.message.color = usageRequirementHighEnoughColor;
                        }
                        else
                        {
                            row.message.color = usageRequirementToLowColor;
                        }
                    }
                }
            }

            box.transform.SetParent(container);
            InventoryUtility.ResetTransform(box.transform);
        }
    }
}