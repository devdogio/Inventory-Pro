using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [CreateAssetMenu(menuName = InventoryPro.CreateAssetMenuPath + "Item drop handler")]
    public class ItemDropHandler : ItemDropHandlerBase
    {
        /// <summary>
        /// Drop an object at the cursor / mouse position position
        /// If false, the item will be dropped using the offset vector.
        /// </summary>
        public bool dropAtMousePosition = true;

        /// <summary>
        /// The offset by which an item is dropped into the world
        /// Only used when dropAtMousePosition = false;
        /// </summary>
        public Vector3 dropOffsetVector = new Vector3(0.0f, 0.0f, 0.0f);

        /// <summary>
        /// The maxmimum distance an item can be dropped
        /// </summary>
        public float maxDropDistance = 10.0f;

        /// <summary>
        /// Layers to consider when ray casting for item dropping.
        /// </summary>
        public LayerMask layersWhenDropping = -1;

        /// <summary>
        /// When dropping an item, do you want it placed precisely on the ground?
        /// </summary>
        public bool dropItemRaycastToGround = false;


        protected ItemDropHandler()
        {
            
        }


        protected virtual Vector3 GetItemDropPosition(InventoryItemBase item)
        {
            return PlayerManager.instance.currentPlayer.transform.position;
        }

        protected virtual Quaternion GetItemDropRotation(InventoryItemBase item)
        {
            return PlayerManager.instance.currentPlayer.transform.rotation;
        }

        public sealed override GameObject DropItem(InventoryItemBase item)
        {
            return DropItem(item, GetItemDropPosition(item), GetItemDropRotation(item));
        }

        public override GameObject DropItem(InventoryItemBase item, Vector3 position, Quaternion rotation)
        {
            if (item.isDroppable == false ||
                (item.itemCollection != null && item.itemCollection.canDropFromCollection == false))
            {
                return null;
            }

            var dropObj = CreateDropObject(item);
            var dropPos = CalculateDropPosition(item, position, rotation);
            dropObj.transform.SetParent(null); // Drop item into the world
            dropObj.transform.position = dropPos;
            dropObj.layer = InventorySettingsManager.instance.settings.itemWorldLayer;
            dropObj.SetActive(true);

            item.NotifyItemDropped(dropObj);

            return dropObj;
        }

        protected virtual GameObject CreateDropObject(InventoryItemBase item)
        {
            GameObject dropObj = item.gameObject;
            if (item.overrideDropObjectPrefab != null)
            {
                // Drop override drop object
                dropObj = Instantiate<GameObject>(item.overrideDropObjectPrefab);
                var trigger = dropObj.GetOrAddComponent<ItemTrigger>();
                trigger.itemPrefab = item;
            }
            else if (item.rarity != null && item.rarity.dropObject != null)
            {
                // Drop a specific item whenever this is dropped
                dropObj = Instantiate<GameObject>(item.rarity.dropObject);
                var holder = dropObj.GetOrAddComponent<ItemTrigger>();
                holder.itemPrefab = item;
            }

            dropObj.GetOrAddComponent<ItemTrigger>();
            if (dropObj.GetComponent<ITriggerInputHandler>() == null)
            {
                dropObj.AddComponent<ItemTriggerInputHandler>();
            }
            return dropObj;
        }

        public sealed override Vector3? CalculateDropPosition(InventoryItemBase item)
        {
            return CalculateDropPosition(item, GetItemDropPosition(item), GetItemDropRotation(item));
        }

        protected virtual Vector3 CalculateDropPosition(InventoryItemBase item, Vector3 initialLocation, Quaternion initialRotation)
        {
            if (dropAtMousePosition)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, maxDropDistance, layersWhenDropping))
                {
                    return hit.point;
                }
            }

            initialLocation += (initialRotation * dropOffsetVector);

            float droppableFromDistanceUp = 10.0f;
            if (dropItemRaycastToGround)
            {
                // If there is something above the item, we can't move it up to raycast down, as this would place it on the collider above it. So first check how much we can go up...
                RaycastHit aboveHit;
                if (Physics.Raycast(initialLocation, Vector3.up, out aboveHit, 10.0f))
                {
                    float dist = Vector3.Distance(aboveHit.transform.position, initialLocation);
                    droppableFromDistanceUp = Mathf.Clamp(dist - 0.1f, 0.1f, 10.0f); // Needs to be at least a little above the ground
                }

                RaycastHit hit;
                if (Physics.Raycast(initialLocation + (Vector3.up * droppableFromDistanceUp), Vector3.down, out hit, 25.0f))
                {
                    initialLocation = hit.point + (Vector3.up * 0.1f); // + a little offset to avoid it falling through the ground
                }
            }

            return initialLocation;
        }
    }
}
