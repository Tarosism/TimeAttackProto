using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{
    public class CustomCharacterInventory : CharacterInventory
    {
        [Header("Secondary Weapons")]
        [Tooltip("the name of the secondary weapon inventory")]
        public string SecondaryWeaponInventoryName;

        public Inventory SecondaryWeaponInventory { get; set; }

        protected override void GrabInventories()
        {
            base.GrabInventories();

            // Grabbing secondary weapon inventory
            if (SecondaryWeaponInventory == null)
            {
                GameObject secondaryWeaponInventoryTmp = GameObject.Find(SecondaryWeaponInventoryName);
                if (secondaryWeaponInventoryTmp != null)
                {
                    SecondaryWeaponInventory = secondaryWeaponInventoryTmp.GetComponent<Inventory>();
                }
            }

            if (SecondaryWeaponInventory != null)
            {
                SecondaryWeaponInventory.SetOwner(this.gameObject);
                SecondaryWeaponInventory.TargetTransform = InventoryTransform;
            }
        }

        // 여기에 secondary weapon inventory와 관련된 추가 메서드 및 로직을 추가할 수 있습니다.
    }
}

