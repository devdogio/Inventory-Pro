using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;
using UnityEngine.UI;
using Devdog.General.UI;
using UnityEngine.Serialization;

namespace Devdog.InventoryPro
{
    [RequireComponent(typeof(UIWindow))]
    public partial class SelectableObjectInfoUI : MonoBehaviour
    {
        [SerializeField]
        private Text _objectName;

        [Header("Health")]
        [SerializeField]
        private Slider _objectHealthSlider;

        [SerializeField]
        private RectTransform _healthContainer; // Parent of health

        [SerializeField]
        private Text _objectHealth;

        [SerializeField]
        private Text _maxObjectHealth;


        private ISelectableObjectInfo _currentSelectableObject;
        public ISelectableObjectInfo currentSelectableObject
        {
            get { return _currentSelectableObject; }
            set
            {
                _currentSelectableObject = value;
                Repaint(_currentSelectableObject);
            }
        }

        public UIWindow window
        {
            get { return GetComponent<UIWindow>(); }
        }

        protected virtual void Start()
        {
            TriggerManager.OnCurrentTriggerChanged += OnTriggerChanged;
        }

        protected virtual void OnDestroy()
        {
            TriggerManager.OnCurrentTriggerChanged -= OnTriggerChanged;
        }

        protected virtual void OnTriggerChanged(TriggerBase before, TriggerBase after) 
        {
            if (after != null)
            {
                var info = after.GetComponent<ISelectableObjectInfo>();
                if (info != null)
                {
                    currentSelectableObject = info;
                }
            }
            else
            {
                currentSelectableObject = null;
            }
        }

        public void Repaint(ISelectableObjectInfo objectInfo)
        {
            if (objectInfo != null)
                window.Show();
            else
            {
                window.Hide();
                return;
            }

            if(_objectName != null)
                _objectName.text = objectInfo.name;

            if (objectInfo.useHealth)
            {
                _healthContainer.gameObject.SetActive(true);

                if (_objectHealthSlider != null)
                {
                    _objectHealthSlider.value = objectInfo.healthFactor;
                }

                if (_objectHealth != null)
                {
                    _objectHealth.text = objectInfo.health.ToString();
                }

                if (_maxObjectHealth != null)
                {
                    _maxObjectHealth.text = objectInfo.maxHealth.ToString();
                }
            }
            else
            {
                _healthContainer.gameObject.SetActive(false);
            }
        }
    }
}