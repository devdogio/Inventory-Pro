using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Devdog.InventoryPro.Editors
{
    [CustomEditor(typeof(PlayerPrefsCollectionSaverLoader))]
    public class PlayerPrefsCollectionSaverLoaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Delete save data"))
            {
                var t = (PlayerPrefsCollectionSaverLoader)target;
                PlayerPrefs.DeleteKey(t.saveName);

                Debug.Log("Save data for key : " + t.saveName + " deleted", t);
            }
        }
    }
}