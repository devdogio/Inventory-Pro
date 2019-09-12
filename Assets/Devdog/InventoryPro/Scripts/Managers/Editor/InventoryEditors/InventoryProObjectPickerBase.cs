using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.General.Editors;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Devdog.InventoryPro.Editors
{
    public class InventoryProObjectPickerBase : ObjectPickerBaseEditor
    {
        public override bool IsSearchMatch(Object asset, string searchQuery)
        {
            searchQuery = searchQuery.ToLower();

            return GetObjectName(asset).ToLower().Contains(searchQuery) ||
                   asset.GetType().Name.ToLower().Contains(searchQuery);
        }

        protected override IEnumerable<Object> FindAssetsOfType(Type type, bool allowInherited)
        {
            var objs = base.FindAssetsOfType(type, allowInherited).ToList();
            for (int i = objs.Count - 1; i >= 0; i--)
            {
                var o = objs[i];
                var path = AssetDatabase.GetAssetPath(o);
                if (string.IsNullOrEmpty(path) || path.Contains(InventoryScriptableObjectUtility.prefabsSaveFolder) == false)
                {
                    objs.RemoveAt(i);
                }
            }

            return objs;
        }

        protected override IEnumerable<Object> FindAssetsWithComponent(Type type, bool allowInherited)
        {
            var objs = base.FindAssetsWithComponent(type, allowInherited).ToList();
            for (int i = objs.Count - 1; i >= 0; i--)
            {
                var o = objs[i];
                var path = AssetDatabase.GetAssetPath(o);
                if (string.IsNullOrEmpty(path) || path.Contains(InventoryScriptableObjectUtility.prefabsSaveFolder) == false)
                {
                    objs.RemoveAt(i);
                }
            }

            return objs;
        }
    }
}
