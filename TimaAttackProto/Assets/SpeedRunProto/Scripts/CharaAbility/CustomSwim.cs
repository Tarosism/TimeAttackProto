using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
public class CustomSwim : CharacterSwim
{
    private void Update()
    {
        if (InWater)
        {
            // 헤엄치는 상태에서 플레이어와 플랫폼 간의 충돌을 무시
            Physics2D.IgnoreLayerCollision(LayerManager.PlayerLayerMask, LayerManager.PlatformsLayerMask, true);
        }
        else
        {
            // 헤엄치는 상태가 아닐 때 원래의 충돌 설정 복원
            Physics2D.IgnoreLayerCollision(LayerManager.PlayerLayerMask, LayerManager.PlatformsLayerMask, false);
        }
    }
}