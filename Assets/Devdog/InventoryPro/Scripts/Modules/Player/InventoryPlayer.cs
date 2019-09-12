using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Player/Inventory player")]
    [RequireComponent(typeof(Devdog.General.Player))]
    public partial class InventoryPlayer : InventoryPlayerBase, IEquippableCharacter
    {
//        public delegate void PickedUpItem(uint itemID, uint itemAmount);
        public delegate void PlayerDied(LootableObject dropObject);

//        public event PickedUpItem OnPickedUpItem;
        public event PlayerDied OnPlayerDied;


        private StatsCollection _stats = new StatsCollection();
        public StatsCollection stats
        {
            get { return _stats; }
            protected set { _stats = value; }
        }


        [SerializeField]
        private bool isPlayerDynamicallyInstantiated = false;

        public bool dontDestroyOnLoad = true;

        [SerializeField]
        private LootableObject _deathDropObjectPrefab;

        [Required]
        [SerializeField]
        private CharacterEquipmentHandlerBase _equipmentHandler;
        public CharacterEquipmentHandlerBase equipmentHandler
        {
            get { return _equipmentHandler; }
        }

        public ICharacterCollection characterCollection
        {
            get { return characterUI; }
        }

        [SerializeField]
        private uint _waitFramesBeforeInit = 0;


        public bool isInitialized { get; protected set; }
        public Player generalPlayer { get; protected set; }

        /// <summary>
        /// Initialize this player. The player will be added to the players list ( assigned to the InventoryPlayerManager )
        /// </summary>
        public virtual void Init()
        {
            Assert.IsFalse(isInitialized, "Tried to initialize player - Player was already initialized!");
            DevdogLogger.LogVerbose("Initializing InventoryPlayer", this);
            isInitialized = true;

            UpdateEquipLocations(transform);

            stats.dataProviders.Add(new StatsProvider());
            stats.Prepare();

            if (dontDestroyOnLoad)
            {
                gameObject.AddComponent<DontDestroyOnLoad>();
            }

            if (dynamicallyFindUIElements)
            {
                FindUIElements();
            }

            if (characterUI != null)
            {
                characterUI.character = this;
                equipmentHandler.Init(characterUI);
            }

            SetAsActivePlayer();
        }
        
        protected override void Awake()
        {
            base.Awake();
            generalPlayer = GetComponent<Player>();

            if (isPlayerDynamicallyInstantiated == false)
            {
                if (_waitFramesBeforeInit == 0)
                {
                    Init();
                }
                else
                {
                    StartCoroutine(WaitFramesThenInit(_waitFramesBeforeInit));
                }
            }
        }

        private IEnumerator WaitFramesThenInit(uint waitFramesBeforeInit)
        {
            for (int i = 0; i < waitFramesBeforeInit; i++)
            {
                yield return null;
            }

            Init();
        }

        protected override void SetAsActivePlayer()
        {
            InventoryPlayerManager.instance.SetPlayerAsCurrentPlayer(this);
        }

        public LootableObject NotifyPlayerDied(bool dropAll)
        {
            return NotifyPlayerDied(dropAll, dropAll, dropAll, transform.position);
        }

        public LootableObject NotifyPlayerDied(bool clearInventories, bool clearCharacter, bool putAllItemsInDropObject, Vector3 dropPosition)
        {
            var playerCollections = new List<ItemCollectionBase>();
            if (clearInventories)
            {
                playerCollections.AddRange(inventoryCollections);
            }

            if (clearCharacter)
            {
                playerCollections.Add(characterUI);
            }

            LootableObject dropObj = null;
            if (putAllItemsInDropObject)
            {
                var itemsInCols = new List<InventoryItemBase>();
                var currencies = new List<CurrencyDecorator>();
                foreach (var col in playerCollections)
                {
                    itemsInCols.AddRange(col.items.Select(o => o.item).Where(o => o != null));
                    currencies.AddRange(col.currenciesGroup.lookups);
                }

                if (itemsInCols.Count > 0 || currencies.Count > 0)
                {
                    if (_deathDropObjectPrefab != null)
                    {
                        dropObj = Instantiate<LootableObject>(_deathDropObjectPrefab);
                        dropObj.items = itemsInCols.ToArray();
                        dropObj.currencies = currencies.ToArray();

                        dropObj.transform.position = transform.position;
                        dropObj.transform.rotation = Quaternion.identity;
                    }
                }
            }

            foreach (var col in playerCollections)
            {
                col.Clear();
            }

            if (OnPlayerDied != null)
                OnPlayerDied(dropObj);

            return dropObj;
        }

        /// <summary>
        /// For collider based characters
        /// </summary>
        /// <param name="col"></param>
        public virtual void OnTriggerEnter(Collider col)
        {
            TryPickup(col.gameObject);
        }

        /// <summary>
        /// For 2D collider based characters
        /// </summary>
        /// <param name="col"></param>
        public virtual void OnTriggerEnter2D(Collider2D col)
        {
            TryPickup(col.gameObject);
        }

        /// <summary>
        /// Collision pickup attempts
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void TryPickup(GameObject obj)
        {
            // Just for safety in-case the collision matrix isn't set up correctly..
            if (obj.layer == InventorySettingsManager.instance.settings.equipmentLayer)
                return;

            if (InventorySettingsManager.instance.settings.itemTriggerOnPlayerCollision || CanPickupGold(obj))
            {
                var item = obj.GetComponent<ItemTrigger>();
                if (item != null)
                    item.Use(generalPlayer);
            }
        }

        protected virtual bool CanPickupGold(GameObject obj)
        {
            return InventorySettingsManager.instance.settings.alwaysTriggerGoldItemPickupOnPlayerCollision && obj.GetComponent<CurrencyInventoryItem>() != null;
        }
    }
}
