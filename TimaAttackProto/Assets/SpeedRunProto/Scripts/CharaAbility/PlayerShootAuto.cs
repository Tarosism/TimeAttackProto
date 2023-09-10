using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{

    public class PlayerShootAuto : MeleeWeapon
    {
        public float attackRange; // 공격 범위
        private bool canAttack = true; // 공격 가능 상태를 나타내는 변수
        public CharacterHandleWeapon characterHandleWeapon;
        Animator AttackSword;
        public override void Initialization()
        {
            base.Initialization();
            characterHandleWeapon = GetComponentInParent<CharacterHandleWeapon>();
            AttackSword = GetComponentInParent<Character>()._animator; // 이렇게 참조를 설정합니다.

        }
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

        private IEnumerator AttackDelay()
        {
            canAttack = false; // 공격을 시작하면 즉시 다음 공격을 막습니다.
            base.WeaponUse();
            AttackSword.SetTrigger("Sword1"); // Sword1 애니메이션 트리거를 설정
            yield return new WaitForSeconds(TimeBetweenUses); // TimeBetweenUses 만큼 대기
            canAttack = true; // 대기가 끝나면 다시 공격할 수 있습니다.
        }
    }
}
