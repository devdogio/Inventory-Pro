using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventoryPro.Editors
{
    public class CraftingEmptyEditor : EmptyEditor
    {


        public CraftingEmptyEditor(string name, EditorWindow window)
            : base(name, window)
        {
            CreateEditors();
        }


        public void CreateEditors()
        {
            childEditors.Clear();
            if (ItemManager.database != null)
            {
                foreach (var cat in ItemManager.database.craftingCategories)
                {
                    if (cat == null)
                    {
                        continue;
                    }

                    childEditors.Add(new CraftingBlueprintEditor(cat.name + " blueprint", cat.name + " blueprints", window, this, cat));
                }
            }

            childEditors.Add(new CraftingCategoryEditor("Crafting category", "Crafting categories", window, this));
        }

        
        public override string ToString()
        {
            return name;
        }
    }
}
