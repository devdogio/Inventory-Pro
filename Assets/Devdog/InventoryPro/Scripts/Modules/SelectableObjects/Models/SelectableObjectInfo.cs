using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Other/Selectable Object Info")]
    public partial class SelectableObjectInfo : MonoBehaviour, ISelectableObjectInfo
    {
        [SerializeField]
        private string _name;
        public new string name
        {
            get { return _name; }
            set { _name = value; }
        }

        [SerializeField]
        private bool _useHealth = true;
        public bool useHealth
        {
            get { return _useHealth; }
            set { _useHealth = value; }
        }

        [SerializeField]
        private float _health = 100;
        public float health
        {
            get { return _health; }
            set { _health = value; }
        }

        [SerializeField]
        private float _maxHealth = 100;
        public float maxHealth
        {
            get { return _maxHealth; }
            set { _maxHealth = value; }
        }


        public float healthFactor
        {
            get { return health/maxHealth; }
        }

        public bool isDead
        {
            get { return health <= 0; }
        }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            health = maxHealth;
        }

        public void ChangeHealth(float changeBy, bool fireEvents = true)
        {
            health += changeBy;
        }
    }
}
