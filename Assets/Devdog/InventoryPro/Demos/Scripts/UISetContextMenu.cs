using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Devdog.InventoryPro.Demo
{
    [RequireComponent(typeof(Toggle))]
    public partial class UISetContextMenu : MonoBehaviour
    {

        private Toggle t;

        public void Awake()
        {
            t = GetComponent<Toggle>();
            t.onValueChanged.AddListener((bool c) =>
            {
                InventorySettingsManager.instance.settings.useContextMenu = c;
            });
        }



    }
}