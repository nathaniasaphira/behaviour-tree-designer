namespace BehaviourTreeDesigner
{
    public class SynchronousNode : CompositeNode
    {
        private int currentChildIndex;

        protected override void OnStart()
        {
            currentChildIndex = 0;
        }

        protected override State OnUpdate()
        {
            switch (children[currentChildIndex].Update())
            {
                case State.Running:
                    return State.Running;
                case State.Success:
                    currentChildIndex++;
                    break;
                case State.Failure:
                    currentChildIndex++;
                    break;
            }

            return State.Success;
        }
    }
}