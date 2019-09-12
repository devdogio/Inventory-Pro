using System;
using System.Collections.Generic;
using Devdog.General.Editors;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Devdog.InventoryPro.Editors
{
    [CustomObjectPicker(typeof(InventoryItemBase), 10)]
    public class InventoryItemObjectPickerEditor : InventoryProObjectPickerBase
    {
        protected override void DrawObject(Rect r, Object obj)
        {
            if (obj == selectedObject)
            {
                var r2 = r;
                r2.x += 3;
                r2.y += 3;
                r2.width -= 6;
                r2.height -= 6;

                GUI.Label(r2, GUIContent.none, "LightmapEditorSelectedHighlight");
            }

            var item = (InventoryItemBase) obj;
            using (new GroupBlock(r, GUIContent.none, "box"))
            {
                var cellSize = r.width;

                var labelRect = new Rect(0, 0, cellSize, EditorGUIUtility.singleLineHeight);
                GUI.Label(labelRect, GetObjectName(obj));
                labelRect.y += EditorGUIUtility.singleLineHeight;
                GUI.Label(labelRect, obj.GetType().Name.Replace("InventoryItem", ""));
                if (item.rarity != null)
                {
                    labelRect.y += EditorGUIUtility.singleLineHeight;
                    GUI.color = item.rarity.color;
                    GUI.Label(labelRect, item.rarity.name);
                    GUI.color = Color.white;
                }

                var iconSize = Mathf.RoundToInt(cellSize * 0.6f);
                if (item.icon != null)
                {
                    DrawTextureGUI(new Rect(cellSize*0.2f, cellSize*0.4f - innerPadding, iconSize, iconSize), item.icon, new Vector2(iconSize, iconSize));
                }
            }
        }

        protected override string GetObjectName(Object obj)
        {
            var item = (InventoryItemBase) obj;
            return item.name;
        }

        public static void DrawTextureGUI(Rect position, Sprite sprite, Vector2 size)
        {
            Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
                                       sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
            Vector2 actualSize = size;

            actualSize.y *= (sprite.rect.height / sprite.rect.width);
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), sprite.texture, spriteRect);
        }
    }
}
