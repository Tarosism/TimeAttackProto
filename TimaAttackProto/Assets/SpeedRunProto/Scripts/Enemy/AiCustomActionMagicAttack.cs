using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class AiCustomActionMagicAttack : AIAction
    {
        public int NumberOfBeams = 3; // 한 번에 발사할 광선의 수
        public GameObject BeamPrefab; // 광선 프리팹
        public Transform[] BeamSpawnPoints; // 광선이 생성될 위치들

        protected int _numberOfBeamsSpawned = 0;
        protected bool _isAttacking = false; // 공격 중인지를 확인하는 플래그

        public override void Initialization()
        {
            // 여기서 필요한 초기화를 수행합니다.
        }

        public override void PerformAction()
        {
            BeamAttack();
        }

        protected virtual void BeamAttack()
        {
            if (_isAttacking && _numberOfBeamsSpawned < NumberOfBeams)
            {
                int beamsToSpawn = Mathf.Min(NumberOfBeams, BeamSpawnPoints.Length); // 둘 중 작은 값을 선택
                for (int i = 0; i < beamsToSpawn; i++)
                {
                    // 지정된 스폰 지점에서 광선을 생성하고 위로 발사합니다.
                    GameObject beam = Instantiate(BeamPrefab, BeamSpawnPoints[i].position, Quaternion.identity);
                    beam.transform.up = Vector3.up; // 광선이 위로 솟아오르도록 방향을 설정합니다.
                }
                _numberOfBeamsSpawned++;
            }
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _numberOfBeamsSpawned = 0; // 상태 진입시 발사된 광선 수를 재설정
            _isAttacking = true; // 공격 상태를 활성화합니다.
        }

        public override void OnExitState()
        {
            base.OnExitState();
            _isAttacking = false; // 공격 상태를 비활성화합니다.
        }
    }
}