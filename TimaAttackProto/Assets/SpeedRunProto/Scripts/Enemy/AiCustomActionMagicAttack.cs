using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class AiCustomActionMagicAttack : AIAction
    {
        public Transform[] BeamSpawnPoints;
        public MMSimpleObjectPooler BeamObjectPooler;
        protected bool _isAttacking = false;

        public override void Initialization()
        {
            BeamObjectPooler.FillObjectPool();
        }

        public override void PerformAction()
        {
            BeamAttack();
        }

        protected virtual void BeamAttack()
        {
            if (_isAttacking)
            {
                // 모든 스폰 포인트에서 빔을 발사
                for (int i = 0; i < BeamSpawnPoints.Length; i++)
                {
                    GameObject beam = BeamObjectPooler.GetPooledGameObject();
                    if (beam != null)
                    {
                        beam.transform.position = BeamSpawnPoints[i].position;
                        beam.transform.up = Vector2.up;
                        beam.SetActive(true);
                    }
                }

                // 모든 빔을 발사한 후에는 _isAttacking을 false로 설정하여 추가 공격을 방지
                _isAttacking = false;
            }
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _isAttacking = true;
        }

        public override void OnExitState()
        {
            base.OnExitState();
            _isAttacking = false;
        }
    }
}
