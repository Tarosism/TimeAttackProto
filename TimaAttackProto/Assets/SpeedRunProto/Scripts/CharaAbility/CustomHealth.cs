using UnityEngine;
using MoreMountains.CorgiEngine;
using System.Collections.Generic;

public class CustomHealth : Health
{
    public override void Damage(float damage, GameObject instigator, float flickerDuration, float invincibilityDuration, Vector3 damageDirection, List<TypedDamage> typedDamages = null)
    {
        base.Damage(0.001f, instigator, flickerDuration, invincibilityDuration, damageDirection, typedDamages);

        if (instigator.transform.position.x > gameObject.transform.position.x)
        {
            if (!_character.IsFacingRight)
            {
                _character.Flip(true);
            }
        }
        else if (instigator.transform.position.x < gameObject.transform.position.x)
        {
            if (_character.IsFacingRight)
            {
                _character.Flip(true);
            }
        }
        _characterHorizontalMovement.enabled = false;
        Invoke("RecoverMovement", 0.4f);
    }

    void RecoverMovement()
    {
        _characterHorizontalMovement.enabled = true;
    }
}


