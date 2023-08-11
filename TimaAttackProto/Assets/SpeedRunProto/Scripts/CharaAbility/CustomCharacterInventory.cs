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

        protected override void EquipWeapon(string weaponID)
        {
            // 여기에 오버라이드할 로직을 작성합니다.
            // 여기서 weaponID는 itemID 인지 확인해보기
        }


        protected virtual void EquipSecondaryWeapon(string weaponID)
        {
            // Logic to equip a weapon into the secondary weapon inventory
            for (int i = 0; i < MainInventory.Content.Length; i++)
            {
                if (InventoryItem.IsNull(MainInventory.Content[i]))
                {
                    continue;
                }
                if (MainInventory.Content[i].ItemID == weaponID)
                {
                    MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null, SecondaryWeaponInventoryName, MainInventory.Content[i], 0, i, PlayerID);
                    break;
                }
            }
        }

        // 여기에 secondary weapon inventory와 관련된 추가 메서드 및 로직을 추가할 수 있습니다.
    }
}

