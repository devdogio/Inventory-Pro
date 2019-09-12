using UnityEngine;
using System.Collections;
using Devdog.General;

namespace Devdog.InventoryPro.Demo
{
    public class LoadLevelOnTriggerEnter : MonoBehaviour
    {
        public string levelToLoad;


        public void OnTriggerEnter(Collider col)
        {
            LoadLevel();
        }


        public void LoadLevel()
        {
            SceneUtility.LoadScene(levelToLoad);
        }
    }
}