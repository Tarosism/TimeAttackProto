using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class BossPattern01 : AIAction
    {
        private CharacterHandleWeapon _weapon;
        private int _numberOfShoots = 0;

        public override void Initialization()
        {
            base.Initialization();
            _weapon = GetComponentInParent<CharacterHandleWeapon>();
        }

        public override void PerformAction()
        {
            BossPattern();
        }

        private void BossPattern()
        {
            if (_numberOfShoots < 3)
            {
                _weapon.CurrentWeapon.TimeBetweenUses = 0.3f;
                _weapon.ShootStart();
            }
            else
            {

            }

        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _numberOfShoots = 0;
        }

        public override void OnExitState()
        {
            base.OnExitState();
            _weapon.ForceStop();
        }
    }
}
