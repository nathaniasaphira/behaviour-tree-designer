using UnityEngine;

namespace BehaviourTreeDesigner
{
    public class MoveNode : ActionNode
    {
        private Vector2 destination;

        protected override State OnUpdate()
        {
            if (sharedData.ThisObject == null || destination == null)
            {
                return State.Failure;
            }

            Move();
            
            return State.Success;
        }

        private void Move()
        {
            sharedData.ThisObject.transform.position = destination;
        }
    }
}