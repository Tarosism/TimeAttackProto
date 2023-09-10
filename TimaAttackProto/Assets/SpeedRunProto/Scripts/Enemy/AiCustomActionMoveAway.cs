using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class AiCustomActionMoveAway : AIActionMoveAwayFromTarget
    {
        public float detectionRadius;
        private int moveDirection = 0;
        public LayerMask platformLayer;

        protected override void Move()
        {
            if (_brain.Target == null)
            {
                _characterHorizontalMovement.SetHorizontalMove(0f);
                return;
            }

            float distanceToTarget = Mathf.Abs(this.transform.position.x - _brain.Target.position.x);

            if (distanceToTarget >= MinimumDistance)
            {
                _characterHorizontalMovement.SetHorizontalMove(0f);
                return;
            }

            // Only check for walls once when the move starts
            if (moveDirection == 0)
            {
                CheckForWalls();
            }

            _characterHorizontalMovement.SetHorizontalMove(moveDirection);
        }

        void CheckForWalls()
        {
            float rayLength = detectionRadius;
            RaycastHit2D hitLeft = Physics2D.Raycast(this.transform.position, Vector2.left, rayLength, platformLayer);
            RaycastHit2D hitRight = Physics2D.Raycast(this.transform.position, Vector2.right, rayLength, platformLayer);

            if (hitLeft.collider != null)
            {
                moveDirection = 1; // Move right if there is a wall on the left
            }
            else if (hitRight.collider != null)
            {
                moveDirection = -1; // Move left if there is a wall on the right
            }
            else
            {
                if (this.transform.position.x < _brain.Target.position.x)
                {
                    moveDirection = -1; // Default behavior: move away from the player
                }
                else
                {
                    moveDirection = 1; // Default behavior: move away from the player
                }
            }
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _characterHorizontalMovement.MovementSpeed = 10f;
            moveDirection = 0; // Reset move direction when entering the state to ensure wall check on next move
        }

        public override void OnExitState()
        {
            base.OnExitState();
            _characterHorizontalMovement.MovementSpeed = 7f;
            _characterHorizontalMovement?.SetHorizontalMove(0f);
        }
    }
}
