using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class CustomCharaHandleWeapon : CharacterHandleWeapon

    {
        protected CharacterInventory _weaponInventory;
        protected Weapon _storedWeapon;
        public override void Setup()
        {
            base.Setup(); // 부모 클래스의 Setup 호출

            //CharacterInventory 컴포넌트를 찾습니다.
            _weaponInventory = GetComponent<CharacterInventory>();
            if (_weaponInventory.WeaponInventory.Content[0])
            {
                _weaponInventory.WeaponInventory.Content[0].Equip("Player1");
            }
        }
    }
}