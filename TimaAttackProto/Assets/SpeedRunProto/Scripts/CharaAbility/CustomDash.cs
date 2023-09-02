using System.Collections;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class CustomDash : CharacterDash
{
    public LayerMask ignoredLayersDuringDash;  // 무시할 레이어를 여기서 설정

    protected override IEnumerator Dash()
    {
        int layerMask = ignoredLayersDuringDash.value;
        for (int i = 0; i < 32; i++)
        {
            if (i < 0 || i > 31) continue;  // 범위 체크
            if ((layerMask & (1 << i)) != 0)
            {
                Physics2D.IgnoreLayerCollision(gameObject.layer, i, true);
            }
        }
        yield return base.Dash();  // 원래 Dash() 코루틴 실행
    }

    public override void StopDash()
    {
        int layerMask = ignoredLayersDuringDash.value;

        base.StopDash();
        for (int i = 0; i < 32; i++)
        {
            if (i < 0 || i > 31) continue;  // 범위 체크
            if ((layerMask & (1 << i)) != 0)
            {
                Physics2D.IgnoreLayerCollision(gameObject.layer, i, false);
            }
        }
    }

}
