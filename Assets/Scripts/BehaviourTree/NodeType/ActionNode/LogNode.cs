using UnityEngine;

namespace BehaviourTreeDesigner
{
    public class LogNode : ActionNode
    {
        public string message;

        protected override void OnStart()
        {
            Debug.Log($"OnStart {message}");
        }

        protected override void OnFinish()
        {
            Debug.Log($"OnFinish {message}");
        }

        protected override State OnUpdate()
        {
            Debug.Log($"OnUpdate {message}");

            return State.Success;
        }
    }
}
