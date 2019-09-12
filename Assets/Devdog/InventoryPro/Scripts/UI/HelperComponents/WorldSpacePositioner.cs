using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;

namespace Devdog.InventoryPro.UI
{
    [RequireComponent(typeof(Trigger))]
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "UI Helpers/World Space Positioner")]
    public partial class WorldSpacePositioner : MonoBehaviour, ITriggerCallbacks
    {
        public Vector3 windowPositionOffset;
        public Vector3 windowRotationOffset;

        private Trigger _trigger;

        protected void Awake()
        {
            _trigger = GetComponent<Trigger>();
        }

        private void PositionNow(InventoryPlayer player)
        {
            if (_trigger.window.window != null)
            {
                _trigger.window.window.transform.position = transform.position;
                _trigger.window.window.transform.rotation = transform.rotation;
                _trigger.window.window.transform.Rotate(windowRotationOffset);

                //+(transform.forward * windowPositionOffset.z)
                _trigger.window.window.transform.Translate(windowPositionOffset);
            }
        }

        public bool OnTriggerUsed(Player player)
        {
            PositionNow(player.inventoryPlayer);

            return false;
        }

        public bool OnTriggerUnUsed(Player player)
        {

            return false;
        }
    }
}
