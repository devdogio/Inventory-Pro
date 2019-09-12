using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine;

namespace Devdog.InventoryPro
{
    public abstract class ItemEquipmentHandlerBase : ScriptableObject
    {
        [Header("General")]
        public bool resizeItemsToScale1 = true;

        [Header("Skinned meshes")]
        [SerializeField]
        protected bool replaceSkinnedMeshBones = true;
        [SerializeField]
        protected bool forceReplaceAllBones = false;

        [Header("Cloth")]
        [SerializeField]
        protected bool copyCapsuleColliders;
//        [SerializeField]
//        protected bool copySphereColliders;

        protected EquippableInventoryItem CreateDefaultCopy(EquippableInventoryItem item)
        {
            var copy = Instantiate<EquippableInventoryItem>(item);

            // Remove the default components, to prevent the user from looting an equipped item.
            Destroy(copy.GetComponent<ITriggerInputHandler>() as UnityEngine.Component);
            Destroy(copy.GetComponent<TriggerBase>());
            Destroy(copy.GetComponent<InventoryItemBase>());

            var rigid = copy.gameObject.GetComponent<Rigidbody>();
            if (rigid != null)
                rigid.isKinematic = true;

            var rigid2D = copy.gameObject.GetComponent<Rigidbody2D>();
            if (rigid2D != null)
                rigid2D.isKinematic = true;

            var cols = copy.gameObject.GetComponentsInChildren<Collider>(true);
            foreach (var col in cols)
                col.isTrigger = true;

            var cols2D = copy.gameObject.GetComponentsInChildren<Collider2D>(true);
            foreach (var col2D in cols2D)
                col2D.isTrigger = true;

            copy.gameObject.SetActive(true);
            InventoryUtility.SetLayerRecursive(copy.gameObject, InventorySettingsManager.instance.settings.equipmentLayer);

            if (resizeItemsToScale1)
            {
                copy.transform.localScale = Vector3.one;
            }

            return copy;
        }

        protected virtual void HandleSkinnedMeshes(EquippableInventoryItem copy, CharacterEquipmentTypeBinder binder)
        {
            if (binder.equipTransform == null)
            {
                return;
            }

            if (replaceSkinnedMeshBones)
            {
                var itemSkinned = copy.GetComponentInChildren<SkinnedMeshRenderer>();
                if (itemSkinned != null)
                {
                    // It's a skinned mesh, combine the bones of this mesh with the player's bones to sync animations.
                    var playerSkinned = binder.equipTransform.GetComponentInParent<SkinnedMeshRenderer>();
                    if (playerSkinned != null)
                    {
                        itemSkinned.rootBone = binder.rootBone;

                        if (forceReplaceAllBones)
                        {
                            itemSkinned.bones = playerSkinned.bones;
                            DevdogLogger.LogVerbose("Force copied " + itemSkinned.bones.Length + " bones to new skinned mesh", itemSkinned);
                        }
                        else
                        {
                            ReplaceBonesOnTarget(playerSkinned, itemSkinned);
                        }
                    }
                    else
                    {
                        DevdogLogger.LogWarning("Tried to equip skinned item to player, but no skinned mesh renderer found in equip position's parent.", binder.equipTransform);
                    }
                }
            }
        }

        protected virtual void HandleClothMeshes(EquippableInventoryItem copy, CharacterEquipmentTypeBinder binder)
        {
            if (binder.equipTransform == null)
            {
                return;
            }

            var cloth = copy.GetComponent<Cloth>();
            if (cloth != null)
            {
                if (copyCapsuleColliders)
                {
                    cloth.capsuleColliders = binder.equipTransform.GetComponentsInParent<CapsuleCollider>();
                }

                cloth.ClearTransformMotion();
            }
        }

        /// <summary>
        /// The bones on the writeTo target will be replaced.
        /// Any bones that could not be found on the source will be ignored.
        /// </summary>
        /// <param name="source">Used to copy the bones from and replace onto the writeTo target</param>
        /// <param name="writeTo">Bones are replaced on this target</param>
        protected void ReplaceBonesOnTarget(SkinnedMeshRenderer source, SkinnedMeshRenderer writeTo)
        {
            var boneCountBefore = writeTo.bones.Length;
            var newBones = new List<Transform>();

            // Search the writeTo.bones (current ones) in the source and copy over those bones.
            foreach (var t in writeTo.bones)
            {
                var bone = FindRecursive(source.rootBone, t.name);
                if (bone != null)
                {
                    newBones.Add(bone);
                }
            }

            writeTo.bones = newBones.ToArray();
            DevdogLogger.LogVerbose("Merged " + newBones.Count + "/" + boneCountBefore + " bones from " + writeTo.gameObject.name + " to " + source.gameObject.name + " (source has " + source.bones.Length + " bones)", source);
        }

        private Transform FindRecursive(Transform t, string name)
        {
            if (t.name == name)
                return t;

            foreach (Transform child in t)
            {
                var val = FindRecursive(child, name);
                if (val != null)
                {
                    return val;
                }
            }

            return null;
        }


        public abstract EquippableInventoryItem Equip(EquippableInventoryItem item, CharacterEquipmentTypeBinder binder, bool createCopy);
        public abstract void UnEquip(CharacterEquipmentTypeBinder binder, bool deleteItem);
    }
}
