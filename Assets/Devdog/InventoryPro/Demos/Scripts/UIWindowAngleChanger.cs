using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.InventoryPro.Demo
{
    public class UIWindowAngleChanger : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Vector2 _prevPosition;

        public Vector2 angleEffect = new Vector2(20, 20);
//        public bool useScale = false;
//        public Vector2 scaleEffect = new Vector2(0.2f, 0.2f);

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        protected virtual void Start()
        {
            if(InventoryManager.instance != null && InventoryManager.instance.uiRoot != null)
            {
                if(InventoryManager.instance.uiRoot.renderMode != RenderMode.ScreenSpaceCamera)
                {
                    enabled = false; // This effect only works on screen space camera.
                    return;
                }
            }
            
            SetAngleBasedOnPosition();
        }

        protected void Update()
        {
            if (WindowMoved(_prevPosition, _rectTransform.anchoredPosition))
            {
                // Draggable window moved, adjust angle.
                _prevPosition = _rectTransform.anchoredPosition;

                SetAngleBasedOnPosition();
            }
        }

        private bool WindowMoved(Vector2 a, Vector2 b)
        {
            float x = Mathf.Abs(a.x - b.x);
            float y = Mathf.Abs(a.y - b.y);

            return x > 0.1f || y > 0.1f;
        }

        protected virtual void SetAngleBasedOnPosition()
        {

            Vector3[] corners = new Vector3[4];
            _rectTransform.GetWorldCorners(corners);

            float x = corners.Average(o => o.x);
            float y = corners.Average(o => o.y);
            var center = new Vector2(x, y);

//            var normalizedPos = GetNormalizedPosition(center);
//
//            normalizedPos -= (Vector2.one / 2f); // Make center of screen 0,0
//            normalizedPos *= 2f; // To re-normalize after subtraction.
            var normalizedPos = center / 10f;
            _rectTransform.localRotation = Quaternion.Euler(-normalizedPos.y * angleEffect.y, normalizedPos.x * angleEffect.x, 0f);

//            if (useScale)
//            {
//                var normalizedInverse = ((1.0f - normalizedPos.x) + (1.0f - normalizedPos.y)) / 2f;
//                var scale = new Vector2(scaleEffect.x * normalizedInverse, scaleEffect.y * normalizedInverse);
//                _rectTransform.localScale = Vector2.one + scale;
//            }
        }

        /// <summary>
        /// Normalize based on the camera pixel width and height.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private Vector2 GetNormalizedPosition(Vector2 pos)
        {
            var cam = InventoryManager.instance.uiCamera;
            pos.x /= cam.pixelWidth;
            pos.y /= cam.pixelHeight;

            return pos;
        }
    }
}
