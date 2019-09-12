using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Player/Inventory Player spawner")]
    public class InventoryPlayerSpawner : InventoryPlayerBase
    {
        [Required]
        public GameObject playerPrefab;

        public bool spawnOnAwake = true;
        public bool dontDestroyPlayerOnLoad = true;
        public bool movePlayerToSpawnPosition = true;
        public bool overridePlayerCollections = true;
        public bool overrideEquipmentBinders = true;


        /// <summary>
        /// When a character is already found in the scene, should spawning be aborted?
        /// </summary>
        public bool forceSingleCharacter = true;


        protected override void Awake()
        {
            base.Awake();

            if (spawnOnAwake)
            {
                Spawn();
            }
        }

        public virtual void Spawn()
        {
            var foundPlayer = FindObjectOfType<InventoryPlayer>();
            if (forceSingleCharacter && foundPlayer != null)
            {
                UpdateEquipLocations(foundPlayer.transform);

                //                Debug.Log("Inventory Pro player already found in scene, enforcing singel player.");
                if (foundPlayer.isInitialized == false)
                {
                    foundPlayer.Init();
                }

                if (movePlayerToSpawnPosition)
                {
                    foundPlayer.transform.position = transform.position;
                }
                return;
            }

            var playerObj = Object.Instantiate<GameObject>(playerPrefab);
            UpdateEquipLocations(playerObj.transform);
            var player = playerObj.GetComponentInChildren<InventoryPlayer>();
            Assert.IsNotNull(player, "playerPrefab on playerSpawner doesn't contain an InventoryPlayer component!");
            if (dontDestroyPlayerOnLoad)
            {
                DontDestroyOnLoad(playerObj);
            }

            player.transform.root.gameObject.name = playerPrefab.name;
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;

            if (overridePlayerCollections)
            {
                player.characterUI = this.characterUI;
                player.inventoryCollections = this.inventoryCollections;
                player.skillbarCollection = this.skillbarCollection;
            }

            if (overrideEquipmentBinders)
            {
                player.equipmentBinders = equipmentBinders;
            }

            player.Init();

            transform.DetachChildren();
            Destroy(gameObject); // No longer need spawner.
        }
    }
}
