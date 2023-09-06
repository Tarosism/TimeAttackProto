using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class BossPattern01 : AIActionShoot
    {
        public ComboWeapon comboWeapon; // 콤보 무기 인스턴스

        public override void Initialization()
        {
            base.Initialization();
            comboWeapon = GetComponentInParent<ComboWeapon>(); // 콤보 무기 초기화
        }

        protected override void Shoot()
        {
            base.Shoot();
            if (_numberOfShoots < 1)
            {
                TargetHandleWeapon.ShootStart();
                if (comboWeapon != null)
                {
                    Debug.Log("콤보 시작");
                    comboWeapon.WeaponStarted(TargetHandleWeapon.CurrentWeapon); // 콤보 시작
                }
                _numberOfShoots++;
            }
            else
            {
                if (comboWeapon != null)
                {
                    comboWeapon.WeaponStopped(TargetHandleWeapon.CurrentWeapon); // 콤보 종료 후 다음 무기로
                }
            }
        }
    }
}