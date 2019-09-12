using UnityEngine;
using System.Collections;

namespace Devdog.InventoryPro
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}