using UnityEngine;

namespace BehaviourTreeDesigner
{
    public class WaitNode : ActionNode
    {
        public float duration = 1f;
        private float startTime;

        protected override void OnStart()
        {
            startTime = Time.time;
        }

        protected override State OnUpdate()
        {
            return IsTimeUp() ? State.Success : State.Running;
        }

        private bool IsTimeUp()
        {
            return Time.time - startTime > duration;
        }
    }
}
