using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.General.Editors;
using UnityEditor;
using UnityEngine;
using Devdog.InventoryPro;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    public class CurrencyEditor : ScriptableObjectEditorCrud<CurrencyDefinition>
    {
        protected override List<CurrencyDefinition> crudList
        {
            get { return new List<CurrencyDefinition>(ItemManager.database.currencies); }
            set { ItemManager.database.currencies = value.ToArray(); }
        }

        private UnityEditorInternal.ReorderableList _currencyConversionList;


        public CurrencyEditor(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        {}


        public override void EditItem(CurrencyDefinition item, int itemIndex)
        {
            base.EditItem(item, itemIndex);

            _currencyConversionList = new UnityEditorInternal.ReorderableList(item.currencyConversions, typeof(CurrencyConversion), true, true, true, true);
            _currencyConversionList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Currency conversions");
            _currencyConversionList.drawElementCallback += (rect, index, active, focused) =>
                {
                    var r = rect;
                    r.height = 18;
                    r.y += 2;

                    var conversion = item.currencyConversions[index];

                    r.width /= 3;
                    r.width -= 5;
                    EditorGUIUtility.labelWidth = 100;
                    conversion.factor = EditorGUI.FloatField(r, "1 " + item.singleName + " to ", conversion.factor);
                    EditorGUIUtility.labelWidth = EditorStyles.labelWidth;

                    r.x += r.width + 5;

                    //r.width /= 2;
                    ObjectPickerUtility.RenderObjectPickerForType<CurrencyDefinition>(r, string.Empty, conversion.currency, (c) =>
                    {
                        conversion.currency = c;
                    });

                    r.x += r.width + 5;
                    EditorGUIUtility.labelWidth = 80;
                    conversion.useInAutoConversion = EditorGUI.Toggle(r, "auto conv.", conversion.useInAutoConversion);
                    EditorGUIUtility.labelWidth = EditorStyles.labelWidth;

                };
            _currencyConversionList.onAddCallback += list =>
                {
                    var l = new List<CurrencyConversion>(item.currencyConversions);
                    l.Add(new CurrencyConversion());
                    item.currencyConversions = l.ToArray();

                    list.list = item.currencyConversions;
                };
            _currencyConversionList.onRemoveCallback += list =>
                {
                    var l = new List<CurrencyConversion>(item.currencyConversions);
                    l.RemoveAt(list.index);
                    item.currencyConversions = l.ToArray();

                    list.list = item.currencyConversions;
                };

        }
        
        protected override bool MatchesSearch(CurrencyDefinition currency, string searchQuery)
        {
            if (currency == null)
                return false;

            string search = searchQuery.ToLower();
            return (currency.ID.ToString().Contains(search) || currency.singleName.ToLower().Contains(search) || currency.pluralName.ToLower().Contains(search));
        }

        protected override void GiveItemNewID(CurrencyDefinition item)
        {
            item.ID = crudList.Count > 0 ? crudList.Max(o => o.ID) + 1 : 0;
        }

        public override void DuplicateItem(int index)
        {
            var item = Clone(index);
            GiveItemNewID(item);
            item.singleName += "(duplicate)";
            item.pluralName += "(duplicate)";
            AddItem(item);
        }

        protected override void DrawSidebarRow(CurrencyDefinition item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            DrawSidebarRowElement(item.singleName, 260);
            DrawSidebarValidation(item, i);

            EndSidebarRow(item, i);
        }

        protected override void DrawDetail(CurrencyDefinition currency, int itemIndex)
        {
            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);
            EditorGUIUtility.labelWidth = EditorStyles.labelWidth;
            RenameScriptableObjectIfNeeded(currency, currency.singleName);

            EditorGUILayout.LabelField("#" + currency.ID);
            if (currency.singleName == "")
                GUI.color = Color.red;

            currency.singleName = EditorGUILayout.DelayedTextField("Single name", currency.singleName);
            GUI.color = Color.white;

            if (currency.pluralName == "")
                GUI.color = Color.red;

            currency.pluralName = EditorGUILayout.TextField("Plural name", currency.pluralName);
            GUI.color = Color.white;

            currency.description = EditorGUILayout.TextField("Description", currency.description);
            currency.allowFractions = EditorGUILayout.Toggle("Allow fractions", currency.allowFractions);

            ObjectPickerUtility.RenderObjectPickerForType("Icon", currency.icon, typeof(Sprite), val =>
            {
                currency.icon = (Sprite) val;
            });

            GUI.color = Color.yellow;
            EditorGUILayout.LabelField("You can use string.Format elements to define the text formatting of the currency: ", EditorStyles.labelStyle);
            EditorGUILayout.LabelField("{0} = The current amount");
            EditorGUILayout.LabelField("{1} = Min min amount");
            EditorGUILayout.LabelField("{2} = The max amount");
            EditorGUILayout.LabelField("{3} = Single / plural name (depends on amount)");
            GUI.color = Color.white;
            currency.stringFormat = EditorGUILayout.TextField("String format", currency.stringFormat);

            EditorGUILayout.LabelField("Format example (single): ", currency.ToString(1, 0.0f, 10f));
            EditorGUILayout.LabelField("Format example (pural): ", currency.ToString(100f, 0.0f, 100f));

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Conversions", EditorStyles.titleStyle);


            EditorGUILayout.LabelField("Currencies can be converted between one another. For example convert 1 Japanese yen to 0.008 dollars.", EditorStyles.labelStyle);
            if (currency.autoConvertOnMaxCurrency != null &&
                currency.currencyConversions.Any(o => o.currency == currency.autoConvertOnMaxCurrency) == false &&
                currency.autoConvertOnMax)
            {
                EditorGUILayout.HelpBox("Auto convert on max currency " + currency.autoConvertOnMaxCurrency.pluralName + " not in list. Can't convert currency", MessageType.Error);
            }

            if (currency.autoConvertFractionsToCurrency != null &&
                currency.currencyConversions.Any(o => o.currency == currency.autoConvertFractionsToCurrency) == false &&
                currency.autoConvertFractions && currency.allowFractions == false)
            {
                EditorGUILayout.HelpBox("Auto convert on fractions " + currency.autoConvertFractionsToCurrency.pluralName + " not in list. Can't convert currency", MessageType.Error);
            }

            EditorGUILayout.LabelField("Make sure you order conversions based on priority.", EditorStyles.labelStyle);
            _currencyConversionList.DoLayoutList();


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Automatic conversions", EditorStyles.titleStyle);

            GUI.color = Color.yellow;
            EditorGUILayout.LabelField("When a currency hits the maximum amount auto. convert it to another currency.", EditorStyles.labelStyle);
            EditorGUILayout.LabelField("For example, in some games you have copper, silver and gold. You can never have more than 99 coper, because it's converted to silver.", EditorStyles.labelStyle);
            GUI.color = Color.white;

            currency.autoConvertOnMax = EditorGUILayout.Toggle("Auto convert on max", currency.autoConvertOnMax);
            if (currency.autoConvertOnMax)
            {
                currency.autoConvertOnMaxAmount = EditorGUILayout.FloatField("Auto convert on max amount", currency.autoConvertOnMaxAmount);

                if (currency.autoConvertOnMaxCurrency == currency)
                {
                    GUI.color = Color.red;
                }

                ObjectPickerUtility.RenderObjectPickerForType<CurrencyDefinition>("Auto convert on max currency", currency.autoConvertOnMaxCurrency, (val) =>
                {
                    currency.autoConvertOnMaxCurrency = val;
                });

                GUI.color = Color.white;
                if (currency.autoConvertOnMaxCurrency == currency)
                {
                    EditorGUILayout.HelpBox("Can't auto convert to self", MessageType.Error);
                }
            }

            if (currency.allowFractions == false)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUI.color = Color.yellow;
                EditorGUILayout.LabelField("When a fraction is introduced convert it to a lower currency.");
                EditorGUILayout.LabelField("For example, in some games you have copper, silver and gold. When an attempt is done to add 1.1 silver, add 1 silver and 10 copper (converting the fraction to 10 copper).", EditorStyles.labelStyle);
                EditorGUILayout.LabelField("When this option is disabled fractions will be discarded.");
                GUI.color = Color.white;

                currency.autoConvertFractions = EditorGUILayout.Toggle("Auto convert fractions", currency.autoConvertFractions);
                if (currency.autoConvertFractions)
                {
                    ObjectPickerUtility.RenderObjectPickerForType<CurrencyDefinition>("Auto convert fractions to currency", currency.autoConvertFractionsToCurrency, (val) =>
                    {
                        currency.autoConvertFractionsToCurrency = val;
                    });
                }
            }



            EditorGUILayout.EndVertical();


            ValidateItemFromCache(currency);
        }

        protected override bool IDsOutOfSync()
        {
            return false;
        }

        protected override void SyncIDs()
        {

        }
    }
}
