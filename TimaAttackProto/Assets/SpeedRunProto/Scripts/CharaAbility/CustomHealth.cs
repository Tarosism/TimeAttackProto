using UnityEngine;
using MoreMountains.CorgiEngine;
using System.Collections.Generic;

public class CustomHealth : Health
{
    private float _originalMovementSpeed;

    //이속을 조절하기 위해 CharacterHorizontalMovement 컴포넌트의 속도를 가져온다
    protected override void Initialization()
    {
        base.Initialization();
        _characterHorizontalMovement = this.gameObject.GetComponent<CharacterHorizontalMovement>();
        if (_characterHorizontalMovement != null)
        {
            _originalMovementSpeed = _characterHorizontalMovement.MovementSpeed;
        }
    }

    // Damage 메서드를 override
    public override void Damage(float damage, GameObject instigator, float flickerDuration, float invincibilityDuration, Vector3 damageDirection, List<TypedDamage> typedDamages = null)
    {
        // 피격 시 데미지를 낮게 설정
        // 피격 데미지가 없으면 '피격상태'와 관련된 모든 걸 할 수 없어서 아주 낮은 데미지를 준다
        base.Damage(0.001f, instigator, flickerDuration, invincibilityDuration, damageDirection, typedDamages);

        if (_characterHorizontalMovement != null)
        {
            _characterHorizontalMovement.MovementSpeed = 1f;
            Invoke("RestoreMovement", 1f); // 1초 후 움직임 복구, 이 시간은 자유롭게 조정할 수 있습니다.
        }
    }

    //공격을 당한 후 이속이 원래대로 돌아오는 시기
    //웬일로 코루틴 안 썼대?
    private void RestoreMovement()
    {
        if (_characterHorizontalMovement != null)
        {
            _characterHorizontalMovement.MovementSpeed = _originalMovementSpeed;
        }
    }
}
