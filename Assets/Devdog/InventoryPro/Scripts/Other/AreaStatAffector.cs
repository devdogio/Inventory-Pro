using UnityEngine;
using System.Collections;
using Devdog.General;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Actions/Area Stat Affector")]
    public class AreaStatAffector : MonoBehaviour
    {
        public delegate void Change(ICharacterStats characterStats, IStat stat);
        public event Change OnEnter;
        public event Change OnStay;
        public event Change OnExit;

        [SerializeField]
        [ForceCustomObjectPicker]
        [Required]
        private StatDefinition _stat;
        public StatDefinition stat
        {
            get { return _stat; }
            protected set { _stat = value; }
        }

        [Header("Enter")]
        public bool changeOnEnter = true;
        public float enterChangeAmount = 10f;
        
        [Header("Stay")]
        public bool changeOnStay = true;
        public float onStayChangeInterval = 1.0f; // Deal damage every N seconds
        public float onStayChangeAmount = 2f;

        [Header("Exit")]
        public bool changeOnExit = true;
        public float onExitChangeAmount = 2f;
        
        [Header("Audio & Visuals")]
        [SerializeField]
        private General.AudioClipInfo _audioClipOnDamage;

        [SerializeField]
        private GameObject _particleEffect;

        [SerializeField]
        private Vector3 _particleOffset;
        

        private Coroutine _coroutine;
        private WaitForSeconds _onStayWaitForSeconds;

        protected virtual void Awake()
        {
            _onStayWaitForSeconds = new WaitForSeconds(onStayChangeInterval);
        }

        protected void OnTriggerEnter(Collider other)
        {
            var c = other.GetComponentInChildren<ICharacterStats>();
            if (c != null)
            {
                var s = c.stats.Get(this.stat.category, this.stat.statName);
                if (changeOnEnter)
                {
                    ChangeStat(s, other, enterChangeAmount);
                }

                if (changeOnStay)
                {
                    _coroutine = StartCoroutine(_OnStay(c, s, other));
                }

                if (OnEnter != null)
                {
                    OnEnter(c, s);
                }
            }
        }

        protected virtual IEnumerator _OnStay(ICharacterStats character, IStat statToChange, Collider other)
        {
            // Keeps going forever untill StopCoroutine is called.
            while (true)
            {
                yield return _onStayWaitForSeconds;

                ChangeStat(statToChange, other, onStayChangeAmount);
                if (OnStay != null)
                {
                    OnStay(character, statToChange);
                }
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            var c = other.GetComponentInChildren<ICharacterStats>();
            if (c != null)
            {
                var s = c.stats.Get(this.stat);
                if (changeOnExit)
                {
                    ChangeStat(s, other, onExitChangeAmount);
                }

                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }

                if (OnExit != null)
                {
                    OnExit(c, s);
                }
            }
        }

        private void ChangeStat(IStat statToChange, Collider col, float amount)
        {
            if (statToChange == null)
            {
                return; // Character doesn't have this stat
            }

            statToChange.ChangeCurrentValueRaw(amount);
            AudioManager.AudioPlayOneShot(_audioClipOnDamage);

            if (_particleEffect != null)
            {
                var particles = Instantiate(_particleEffect);
                particles.transform.position = col.transform.position + _particleOffset;

                Destroy(particles.gameObject, 1.0f);
            }
        }
    }
}