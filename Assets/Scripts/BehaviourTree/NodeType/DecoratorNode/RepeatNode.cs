using UnityEngine;

namespace BehaviourTreeDesigner
{
    public class RepeatNode : DecoratorNode
    {
        [Header("Repeat Configuration")]
        [SerializeField] private int count;
        [SerializeField] private bool repeatForever;
        //[SerializeField] private bool endOnFailure;

        private int iterationsLeft;

        protected override void OnStart()
        {
            iterationsLeft = count;
        }

        protected override State OnUpdate()
        {
            if (child == null)
            {
                Debug.LogWarning($"[Repeat] Child is null");
                return State.Failure;
            }

            child.Update();
            iterationsLeft--;

            return repeatForever || iterationsLeft > 0? State.Running : State.Success;
        }
    }
}
