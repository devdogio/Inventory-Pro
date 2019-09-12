using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.General.Editors;
using EditorStyles = Devdog.General.Editors.EditorStyles;
using EditorUtility = UnityEditor.EditorUtility;

namespace Devdog.InventoryPro.Editors
{
	[CustomEditor(typeof(InventoryItemBase), true)]
	public class InventoryItemBaseEditor : InventoryEditorBase
	{
		//private InventoryItemBase item;

		protected SerializedProperty id;
		protected SerializedProperty itemName; // Name is used by Editor.name...
		protected SerializedProperty description;
		protected SerializedProperty stats;
		protected SerializedProperty usageRequirements;
		protected SerializedProperty useCategoryCooldown;
		protected SerializedProperty overrideDropObjectPrefab;
		protected SerializedProperty category;
		protected SerializedProperty icon;
		protected SerializedProperty weight;
		protected SerializedProperty layoutSizeCols;
		protected SerializedProperty layoutSizeRows;
		protected SerializedProperty requiredLevel;
		protected SerializedProperty rarity;
		protected SerializedProperty buyPrice;
		protected SerializedProperty sellPrice;
		protected SerializedProperty isDroppable;
		protected SerializedProperty isSellable;
		protected SerializedProperty isStorable;
		protected SerializedProperty maxStackSize;
		protected SerializedProperty cooldownTime;


		private UnityEditorInternal.ReorderableList _statsList;
		private UnityEditorInternal.ReorderableList _usageRequirementList;

		private bool _saved = true;

		public bool saved
		{
			get
			{
				return _saved;
			}

			set
			{
				_saved = value;
			}
		}


		public override void OnEnable()
		{
			base.OnEnable();

            if (target == null)
                return;

            id = serializedObject.FindProperty("_id");
			itemName = serializedObject.FindProperty("_name");
			description = serializedObject.FindProperty("_description");
			stats = serializedObject.FindProperty("_stats");
			usageRequirements = serializedObject.FindProperty("_usageRequirement");
			useCategoryCooldown = serializedObject.FindProperty("_useCategoryCooldown");
			overrideDropObjectPrefab = serializedObject.FindProperty("_overrideDropObjectPrefab");
			category = serializedObject.FindProperty("_category");
			icon = serializedObject.FindProperty("_icon");
			weight = serializedObject.FindProperty("_weight");
			layoutSizeCols = serializedObject.FindProperty("_layoutSizeCols");
			layoutSizeRows = serializedObject.FindProperty("_layoutSizeRows");
			requiredLevel = serializedObject.FindProperty("_requiredLevel");
			rarity = serializedObject.FindProperty("_rarity");
			buyPrice = serializedObject.FindProperty("_buyPrice");
			sellPrice = serializedObject.FindProperty("_sellPrice");
			isDroppable = serializedObject.FindProperty("_isDroppable");
			isSellable = serializedObject.FindProperty("_isSellable");
			isStorable = serializedObject.FindProperty("_isStorable");
			maxStackSize = serializedObject.FindProperty("_maxStackSize");
			cooldownTime = serializedObject.FindProperty("_cooldownTime");


			var t = (InventoryItemBase)target;

			_statsList = new UnityEditorInternal.ReorderableList(serializedObject, stats, true, true, true, true);
			_statsList.drawHeaderCallback += rect => GUI.Label(rect, "Item stats");
			_statsList.elementHeight = 40;
			_statsList.drawElementCallback += (rect, index, active, focused) =>
			{
				DrawItemStatLookup(rect, stats.GetArrayElementAtIndex(index), active, focused, true, true);
			};
			_statsList.onAddCallback += (list) =>
			{
				var l = new List<StatDecorator>(t.stats);
				l.Add(new StatDecorator());
				t.stats = l.ToArray();

				GUI.changed = true; // To save..

				EditorUtility.SetDirty(target);
				serializedObject.ApplyModifiedProperties();
				Repaint();
			};

			_usageRequirementList = new UnityEditorInternal.ReorderableList(serializedObject, usageRequirements, true, true, true, true);
			_usageRequirementList.drawHeaderCallback += rect => GUI.Label(rect, "Usage requirement stats");
			_usageRequirementList.elementHeight = 40;
			_usageRequirementList.drawElementCallback += (rect, index, active, focused) =>
			{
				var element = usageRequirements.GetArrayElementAtIndex(index);
				DrawUsageRequirement(rect, element, active, focused, true);
			};
			_usageRequirementList.onAddCallback += (list) =>
			{
				var l = new List<StatRequirement>(t.usageRequirement);
				l.Add(new StatRequirement());
				t.usageRequirement = l.ToArray();

				GUI.changed = true; // To save..
				EditorUtility.SetDirty(target);
				serializedObject.ApplyModifiedProperties();
				Repaint();
			};
		}

		protected virtual void DrawItemStatLookup(Rect rect, SerializedProperty property, bool isActive, bool isFocused, bool drawRestore, bool drawPercentage)
		{
			InventoryEditorUtility.DrawStatDecorator(rect, property, isActive, isFocused, drawRestore, drawPercentage);
		}

		protected virtual void DrawUsageRequirement(Rect rect, SerializedProperty property, bool isActive, bool isFocused, bool drawFilterType)
		{
			InventoryEditorUtility.DrawStatRequirement(rect, property, isActive, isFocused, drawFilterType);
		}

		private IEnumerator DestroyImmediateThis(InventoryItemBase obj)
		{
			yield return null;
			DestroyImmediate(obj.gameObject, false); // Destroy this object
		}

		protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
		{
			base.OnCustomInspectorGUI(extraOverride);

			if (serializedObject == null || target == null)
			{
				return;
			}

			serializedObject.Update();
			overrides = extraOverride;

			// Can't go below 0
			if (cooldownTime.floatValue < 0.0f)
				cooldownTime.floatValue = 0.0f;

			GUI.color = Color.yellow;
			var item = (InventoryItemBase)target;
			if (item.gameObject.activeInHierarchy && item.rarity != null && item.rarity.dropObject != null)
			{
				if (GUILayout.Button("Convert to drop object"))
				{
					var dropObj = item.rarity.dropObject;
					var dropInstance = (GameObject)PrefabUtility.InstantiatePrefab(dropObj.gameObject);
					var itemTrigger = dropInstance.AddComponent<ItemTrigger>();

					var t = target;
					if (t == null)
					{
						t = PrefabUtility.GetPrefabParent(t);
					}

					string path = AssetDatabase.GetAssetPath(t);
					var asset = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
					if (asset != null)
					{
						itemTrigger.itemPrefab = asset.GetComponent<InventoryItemBase>();

						dropInstance.transform.SetParent(item.transform.parent);
						dropInstance.transform.SetSiblingIndex(item.transform.GetSiblingIndex());
						dropInstance.transform.position = item.transform.position;
						dropInstance.transform.rotation = item.transform.rotation;

						Selection.activeGameObject = itemTrigger.gameObject;

						item.StartCoroutine(DestroyImmediateThis(item));
					}

					return;
				}
			}
			GUI.color = Color.white;

			if (target == null)
				return;

			var excludeList = new List<string>()
			{
				"m_Script",
				id.name,
				itemName.name,
				description.name,
				stats.name,
				usageRequirements.name,
				useCategoryCooldown.name,
				overrideDropObjectPrefab.name,
				category.name,
				icon.name,
				weight.name,
				layoutSizeCols.name,
				layoutSizeRows.name,
				requiredLevel.name,
				rarity.name,
				buyPrice.name,
				sellPrice.name,
				isDroppable.name,
				isSellable.name,
				isStorable.name,
				maxStackSize.name,
				cooldownTime.name,
			};

			GUILayout.Label("Default", EditorStyles.titleStyle);
			EditorGUILayout.BeginVertical(EditorStyles.boxStyle);
			if (FindOverride(id.name) != null)
				GUI.enabled = false;

			EditorGUILayout.LabelField("ID: ", id.intValue.ToString());
			GUI.enabled = true;

			if (FindOverride(itemName.name) != null)
				GUI.enabled = false;

			GUI.SetNextControlName("ItemEditor_itemName");
			Devdog.General.Editors.EditorUtility.EditableLabel(itemName, false, MarkToSave);

			GUI.enabled = true;

			if (FindOverride(description.name) != null)
				GUI.enabled = false;

			Devdog.General.Editors.EditorUtility.EditableLabel(description, true, MarkToSave, "Note, that you can use rich text like <b>asd</b> to write bold text and <i>Potato</i> to write italic text.");


			GUI.enabled = true;

			EditorGUILayout.PropertyField(icon);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Item layout");

			EditorGUILayout.BeginVertical();
			for (int i = 1; i < 7; i++)
			{
				EditorGUILayout.BeginHorizontal();
				for (int j = 1; j < 7; j++)
				{
					if (layoutSizeCols.intValue < j || layoutSizeRows.intValue < i)
						GUI.color = Color.gray;

					var c = new GUIStyle("CN Box");
					c.alignment = TextAnchor.MiddleCenter;
					if (GUILayout.Button(j + " X " + i, c, GUILayout.Width(40), GUILayout.Height(40)))
					{
						layoutSizeCols.intValue = j;
						layoutSizeRows.intValue = i;
					}

					GUI.color = Color.white;
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

			if (item.icon != null)
			{
				var a = item.icon.bounds.size.x / item.icon.bounds.size.y;
				var b = (float)item.layoutSizeCols / item.layoutSizeRows;

				if (Mathf.Approximately(a, b) == false)
				{
					EditorGUILayout.HelpBox("Layout size is different from icon aspect ratio.", MessageType.Warning);
				}
			}


#if RELATIONS_INSPECTOR
            if ( GUILayout.Button("Show relations", GUILayout.ExpandWidth( false ) ) )
            {
                EditorWindow
                    .GetWindow<RelationsInspector.RelationsInspectorWindow>()
                    .GetAPI1
                    .ResetTargets( new[] { t }, typeof( RelationsInspector.InventoryProBackends.ItemBackend ) );
            }
#endif


			EditorGUILayout.EndVertical();

			// Draws remaining items
			GUILayout.Label("Item specific", EditorStyles.titleStyle);
			EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

			foreach (var x in extraOverride)
			{
				if (x.action != null)
					x.action();

				excludeList.Add(x.serializedName);
			}

			DrawPropertiesExcluding(serializedObject, excludeList.ToArray());
			EditorGUILayout.EndVertical();

			#region Properties

			GUILayout.Label("Item stats", EditorStyles.titleStyle);
			GUILayout.Label("You can create stats in the Item editor / Item stats editor");

			EditorGUILayout.BeginVertical();
			_statsList.DoLayoutList();
			EditorGUILayout.EndVertical();


			GUILayout.Label("Usage requirement stats", EditorStyles.titleStyle);
			GUILayout.Label("Add stats the user is required to have in order to use this item.");
			GUILayout.Label("Example: Usage stat of 10 strength means:");
			GUILayout.Label("The user can only use this item if he/she has 10 or more strength.");

			EditorGUILayout.BeginVertical();
			_usageRequirementList.DoLayoutList();
			EditorGUILayout.EndVertical();

			#endregion

			GUILayout.Label("Behavior", EditorStyles.titleStyle);
			EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

			GUILayout.Label("Details", EditorStyles.titleStyle);
			if (rarity.objectReferenceValue != null)
			{
				var color = ((ItemRarity)rarity.objectReferenceValue).color;
				color.a = 1.0f; // Ignore alpha in the editor.
				GUI.color = color;
			}

			ObjectPickerUtility.RenderObjectPickerForType<ItemRarity>(rarity);
			GUI.color = Color.white;

			ObjectPickerUtility.RenderObjectPickerForType<ItemCategory>(category);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(useCategoryCooldown);
			if (useCategoryCooldown.boolValue)
			{
				if (category.objectReferenceValue != null)
				{
					var c = (ItemCategory)category.objectReferenceValue;
					EditorGUILayout.LabelField(string.Format("({0} seconds)", c.cooldownTime));
				}
			}

			EditorGUILayout.EndHorizontal();
			if (useCategoryCooldown.boolValue == false)
			{
				EditorGUILayout.PropertyField(cooldownTime);
			}


			EditorGUILayout.PropertyField(overrideDropObjectPrefab);

			GameObject dropPrefab = null;
			if (overrideDropObjectPrefab.objectReferenceValue != null)
			{
				EditorGUILayout.HelpBox("Overriding drop object to: " + overrideDropObjectPrefab.objectReferenceValue.name, MessageType.Info);
				dropPrefab = (GameObject)overrideDropObjectPrefab.objectReferenceValue;
			}
			else if (item.rarity != null && item.rarity.dropObject != null)
			{
				EditorGUILayout.HelpBox("Using rarity drop object: " + item.rarity.dropObject.name, MessageType.Info);
				dropPrefab = item.rarity.dropObject;
			}
			else
			{
				EditorGUILayout.HelpBox("No drop object set.", MessageType.Info);
				dropPrefab = item.gameObject;
			}

			if (dropPrefab.GetComponentsInChildren<Collider>(true).Any(o => o.isTrigger) == false && dropPrefab.GetComponentsInChildren<Collider2D>(true).Any(o => o.isTrigger) == false)
			{
				EditorGUILayout.HelpBox("Drop object has no triggers and therefore can never be picked up!", MessageType.Error);
			}


			GUILayout.Label("Buying & Selling", EditorStyles.titleStyle);
			//            EditorGUILayout.BeginHorizontal();
			//            EditorGUILayout.LabelField("Buy price", GUILayout.Width(EditorStyles.labelWidth));
			EditorGUILayout.PropertyField(buyPrice);
			//            EditorGUILayout.EndHorizontal();

			//            EditorGUILayout.BeginHorizontal();
			//            EditorGUILayout.LabelField("Sell price", GUILayout.Width(EditorStyles.labelWidth));
			EditorGUILayout.PropertyField(sellPrice);
			//            EditorGUILayout.EndHorizontal();

			GUILayout.Label("Restrictions", EditorStyles.titleStyle);
			EditorGUILayout.PropertyField(isDroppable);
			EditorGUILayout.PropertyField(isSellable);
			EditorGUILayout.PropertyField(isStorable);
			EditorGUILayout.PropertyField(maxStackSize);
			EditorGUILayout.PropertyField(weight);
			EditorGUILayout.PropertyField(requiredLevel);

			EditorGUILayout.EndVertical();


			serializedObject.ApplyModifiedProperties();
		}

		private void MarkToSave()
		{
			_saved = false;
		}
	}
}