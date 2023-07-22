using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/SuperJump")]
    public class SuperJump : CharacterJump
    {
        // Animation parameters
        protected const string _superJumpParameterName = "SuperJump";
        protected int _superJumpAnimationParameter;

        protected override void Initialization()
        {
            base.Initialization();
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();
        }

        protected override void HandleInput()
        {
            // "z" key로 슈퍼 점프 실행
            if (Input.GetKeyDown(KeyCode.CapsLock))
            {
                DoSuperJump();
            }
        }

        protected virtual void DoSuperJump()
        {
            // 슈퍼 점프를 실행할 수 있는 조건들을 체크
            if (!AbilityPermitted
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                || (!_controller.State.IsGrounded)
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            {
                return;
            }

            // 슈퍼 점프를 실행합니다. 10은 y축으로의 점프 힘을 나타냅니다.
            _controller.SetVerticalForce(50);
        }

        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_superJumpParameterName, AnimatorControllerParameterType.Bool, out _superJumpAnimationParameter);
        }
    }
}
