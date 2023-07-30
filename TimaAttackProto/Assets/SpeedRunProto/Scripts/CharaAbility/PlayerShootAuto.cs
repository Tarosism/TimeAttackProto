using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{

    public class PlayerShootAuto : MeleeWeapon
    {

        public float attackRange = 1f; // 공격 범위
        private bool canAttack = true; // 공격 가능 상태를 나타내는 변수


        protected override void Update()
        {
            // 플레이어 주변의 적을 감지
            Collider2D[] enemies = Physics2D.OverlapCircleAll(base.transform.position, attackRange, LayerManager.EnemiesLayerMask);
            // 감지된 적이 있으면 공격
            if (enemies.Length > 0 && canAttack)
            {
                StartCoroutine(AttackDelay());
            }
        }

        protected void Attack()
        {
            if (base._damageArea == null)
            {
                base.CreateDamageArea();
                // Set ApplyDamageOnTriggerStay to false
                base._damageOnTouch.ApplyDamageOnTriggerStay = false;
            }
            base.WeaponUse();
        }

        private IEnumerator AttackDelay()
        {
            canAttack = false; // 공격을 시작하면 즉시 다음 공격을 막습니다.
            Attack(); // 공격
            yield return new WaitForSeconds(TimeBetweenUses); // TimeBetweenUses 만큼 대기
            canAttack = true; // 대기가 끝나면 다시 공격할 수 있습니다.
        }
    }
}
