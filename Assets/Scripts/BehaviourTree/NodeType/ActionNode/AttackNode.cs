using System;
using UnityEngine;

namespace BehaviourTreeDesigner
{
    public class AttackNode : ActionNode
    {
        protected override State OnUpdate()
        {
            if (sharedData.ThisObject == null)
            {
                Debug.Log("[Attack] ThisObject is null!");
                return State.Failure;
            }

            Attack();
            
            return State.Success;
        }

        private void Attack()
        {
            throw new NotImplementedException();
        }
    }
}
