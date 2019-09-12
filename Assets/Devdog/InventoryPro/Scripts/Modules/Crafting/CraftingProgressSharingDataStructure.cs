using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    public enum CraftingProgressShareSetting
    {
        /// <summary>
        /// The progress will be stored on the crafting window, unless overriden in the trigger.
        /// </summary>
        Default = 0,
        /// <summary>
        /// When enabled the progress will be stored per crafting category, allowing the user to craft 1 item at a time per category.
        /// </summary>
        ForceSingleInstancePerCategory = 1,
        /// <summary>
        /// When enabled only 1 crafting action can be active at a time.
        /// </summary>
        ForceSingleInstance = 2
    }

    public class CraftingProgressSharingDataStructure<T> where T : CraftingProgressContainer
    {
        public delegate T CreateNewInstance(ICraftingActionValidator validator, int id);

        private static T _singleInstance;
//        private static List<T> _list;
        private static Dictionary<int, List<T>> _sharedDict;
        private static CreateNewInstance _createNewAction;

        public CraftingProgressSharingDataStructure(CreateNewInstance createNewAction, T singleInstance, int reserve = 0)
        {
            _createNewAction = createNewAction;
            _singleInstance = singleInstance;

//            _list = new List<T>(reserve);
            _sharedDict = new Dictionary<int, List<T>>(reserve);
        }

        public bool IsShared(CraftingProgressShareSetting setting)
        {
            return setting == CraftingProgressShareSetting.Default || setting == CraftingProgressShareSetting.ForceSingleInstancePerCategory;
        }

        public T Add(T item, int category, CraftingProgressShareSetting shareSetting)
        {
            if (IsShared(shareSetting))
            {
                if(_sharedDict.ContainsKey(category) == false)
                    _sharedDict.Add(category, new List<T>());

                _sharedDict[category].Add(item);
            }
            else
            {
//                _list.Add(item);
            }

            return item;
        }


        public CraftingProgressContainer Get(ICraftingActionValidator validator, int instanceID, int category, CraftingProgressShareSetting shareSetting)
        {
            if (IsShared(shareSetting))
            {
                if (_sharedDict.ContainsKey(category))
                {
                    if (shareSetting == CraftingProgressShareSetting.ForceSingleInstancePerCategory)
                    {
                        if (_sharedDict[category].Count > 0)
                            return _sharedDict[category][0];

//                        foreach (var i in _sharedDict[category])
//                        {
//                            return i;
//                        }
                    }

                    if (shareSetting == CraftingProgressShareSetting.Default)
                    {
                        foreach (var i in _sharedDict[category])
                        {
                            if (i.instanceID == instanceID)
                            {
                                return i;
                            }
                        }
                    }
                }
            }
            else
            {
                return _singleInstance;
            }

            return Add(_createNewAction(validator, instanceID), category, shareSetting);
//            return Get(hash, category, shareSetting);
        }


        public void Remove(int instanceID, int category, CraftingProgressShareSetting shareSetting)
        {
            if (IsShared(shareSetting))
            {
                if (_sharedDict.ContainsKey(category))
                {
                    _sharedDict[category].RemoveAll(o => o.instanceID == instanceID);
                }
            }
            else
            {
//                _list.RemoveAll(o => o.instanceID == instanceID);
            }
        }

        public T[] ToArray(int instanceID, int category, CraftingProgressShareSetting shareSetting)
        {
            if (IsShared(shareSetting))
            {
                return _sharedDict[category].ToArray();
            }

            return new [] { _singleInstance };
//            return _list.ToArray();
        }
    }
}
