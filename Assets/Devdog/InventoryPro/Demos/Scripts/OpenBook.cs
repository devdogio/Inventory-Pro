using UnityEngine;
using System.Collections;
using Devdog.General.UI;

namespace Devdog.InventoryPro
{
    public class OpenBook : MonoBehaviour
    {

        public AnimationClip openAnimation;
        public KeyCode openBookKeyCode = KeyCode.F;

        public UIWindow[] toShow = new UIWindow[0];

        private bool _active;
        private Animator _animator;
        
        public void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(openBookKeyCode))
            {
                _active = !_active;
                _animator.SetFloat("speed", _active ? 1.0f : -1.0f);
                _animator.Play(openAnimation.name);

                if (_active)
                {
                    foreach (var uiWindow in toShow)
                    {
                        uiWindow.Show();
                    }
                }
                else
                {
                    foreach (var uiWindow in toShow)
                    {
                        uiWindow.Hide(0.5f);
                    }
                }
            }
        }
    }
}