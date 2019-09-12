using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;

namespace Devdog.InventoryPro.Demo
{
    public partial class InventoryLookAtPlayer : MonoBehaviour
    {
        public Vector3 rotationOffset;

        public void Update()
        {
            if (PlayerManager.instance.currentPlayer == null)
                return;

            transform.LookAt(PlayerManager.instance.currentPlayer.transform, Vector3.up);
            transform.Rotate(rotationOffset);            
        }

    }
}
