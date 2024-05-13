using System;
using UnityEngine;

namespace BehaviourTreeDesigner
{
    public class WithinRangeNode : ConditionalNode
    {
        protected override State OnUpdate()
        {
            condition = IsTargetWithinRange();
            return condition ? State.Success : State.Failure;
        }
        
        private bool IsTargetWithinRange()
        {
            throw new NotImplementedException();
        }
    }
}
