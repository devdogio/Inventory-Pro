using System;
using System.Collections;
using Devdog.General;
using Devdog.General.UI;
using UnityEngine;

#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif

namespace Devdog.InventoryPro.Demo
{
    [RequireComponent(typeof(SelectableObjectInfo))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(LootableObject))]
    [RequireComponent(typeof(Trigger))]
    public partial class MyCustomMonster : MonoBehaviour, ICharacterStats
    {
//        [SerializeField]
//        private CharacterEquipmentTypeBinder[] _equipmentBinders;
//        public CharacterEquipmentTypeBinder[] equipmentBinders
//        {
//            get { return _equipmentBinders; }
//            set { _equipmentBinders = value; }
//        }
//
//        [SerializeField]
//        private CharacterEquipmentHandlerBase _equipmentHandler;
//        public CharacterEquipmentHandlerBase equipmentHandler
//        {
//            get { return _equipmentHandler; }
//            private set { _equipmentHandler = value; }
//        }

        [SerializeField]
        private StatDefinition[] _startStats = new StatDefinition[0];

        private StatsCollection _stats = new StatsCollection();
        public StatsCollection stats
        {
            get { return _stats; }
            private set { _stats = value; }
        }


        public bool droppedLoot { get; protected set; }

        public float walkSpeed = 4.0f;
        public float walkRadius = 10.0f;

        public bool useLootWindow = true;

        public GameObject corpseParticleEffectPrefab;

        [NonSerialized]
        private WaitForSeconds waitTime = new WaitForSeconds(4.0f);

        [NonSerialized]
        private Vector3 aimPosition;

        [NonSerialized]
        private NavMeshAgent agent;

        public LootableObject lootable { get; protected set; }


        private ISelectableObjectInfo _selectableObjectInfo;

        public ISelectableObjectInfo selectableObjectInfo
        {
            get
            {
                if (_selectableObjectInfo == null)
                {
                    _selectableObjectInfo = GetComponent<ISelectableObjectInfo>();
                }

                return _selectableObjectInfo;
            }
        }

        public Trigger trigger { get; protected set; }


        public bool isDead
        {
            get { return selectableObjectInfo.health <= 0; }
        }

        public void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = walkSpeed;

            droppedLoot = false;

            foreach (var statDef in _startStats)
            {
                stats.Add(statDef.category, new Stat(statDef));
            }

            trigger = GetComponent<Trigger>();
            trigger.window = new UIWindowField() { window = InventoryManager.instance.loot.window };
            trigger.enabled = false; // Only need it once the monster dies
            InventoryManager.instance.loot.window.OnHide += OnLootWindowHide;

            lootable = gameObject.GetComponent<LootableObject>();
            if (lootable == null)
            {
                Debug.LogWarning("No lootable object found on MyCustomMonster (manually added one at runtime to prevent errors)", gameObject);
                lootable = gameObject.AddComponent<LootableObject>();
            }

            StartCoroutine(_ChooseNewLocation());
        }

        private void OnRemovedItem(InventoryItemBase item, uint itemid, uint slot, uint amount)
        {
            lootable.items[slot] = null;
        }

        private void OnLootWindowHide()
        {
            if (useLootWindow)
            {
                InventoryManager.instance.loot.window.OnHide -= OnLootWindowHide; // No longer need this.
                UnSelected();
            }
        }

        protected void UnSelected()
        {
            InventoryManager.instance.loot.OnRemovedItem -= OnRemovedItem; // Un-register callback
        }

        private IEnumerator _ChooseNewLocation()
        {
            while (true)
            {
                ChooseNewLocation();

                yield return waitTime;
            }
        }

        public virtual void ChooseNewLocation()
        {
            if (isDead)
                return;

            aimPosition = UnityEngine.Random.insideUnitCircle * walkRadius;
            agent.SetDestination(transform.position + aimPosition);
        }

        public void OnMouseDown()
        {
            var player = PlayerManager.instance.currentPlayer.inventoryPlayer;
            int dmg = 0;

            if (player != null)
                dmg = 40 + (int)player.stats.Get("Default", "Strength").currentValue;
            else
                dmg = 40;

            Debug.Log("Damage dealt: " + dmg);
            selectableObjectInfo.ChangeHealth(-dmg);

            if (isDead)
                Die(); // Ah it died!
        }

        protected virtual void Die()
        {
            if (!isDead || droppedLoot)
                return; // not actually dead?

            Debug.Log("You killed it!");

            if (corpseParticleEffectPrefab != null)
            {
                var copy = GameObject.Instantiate<GameObject>(corpseParticleEffectPrefab);
                copy.transform.SetParent(transform);
                copy.transform.localPosition = Vector3.zero;
            }

            if (useLootWindow)
            {
                trigger.enabled = true;
            }

            droppedLoot = true;

            agent.Stop();

            StartCoroutine(SinkIntoGround());

            DropLoot();
        }

        protected virtual IEnumerator SinkIntoGround()
        {
            yield return new WaitForSeconds(4.0f * ((useLootWindow) ? 2.0f : 1.0f));
            agent.enabled = false; // To allow for sinking
            float timer = 0.0f;

            while (timer < 3.0f)
            {
                yield return null;

                transform.Translate(0, -1.0f * Time.deltaTime, 0.0f);
                timer += Time.deltaTime;
            }

            Destroy(gameObject); // And clean up.
        }

        public virtual void DropLoot()
        {
            if (useLootWindow)
                return; // Nope, using a loot window

            foreach (var item in lootable.items)
            {
                InventoryItemBase dropItem = Instantiate<InventoryItemBase>(item);
                GameObject drop = dropItem.Drop(transform.position, Quaternion.identity);

                if (drop != null)
                {
                    Rigidbody body = drop.GetComponent<Rigidbody>();
                    if (body != null)
                        body.velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), 3f, UnityEngine.Random.Range(-1f, 1f));

                }
            }
        }
    }
}
