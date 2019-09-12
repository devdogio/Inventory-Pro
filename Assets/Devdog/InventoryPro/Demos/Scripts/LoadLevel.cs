using UnityEngine;
using System.Collections;
using Devdog.General;

namespace Devdog.InventoryPro.Demo
{
    public class LoadLevel : MonoBehaviour
    {
        public void LoadALevel(string level)
        {
            SceneUtility.LoadScene(level);
        }
    }
}