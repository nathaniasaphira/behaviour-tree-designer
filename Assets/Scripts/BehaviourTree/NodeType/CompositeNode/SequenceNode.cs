namespace BehaviourTreeDesigner
{
    public class SequenceNode : CompositeNode
    {
        private int currentChildIndex;

        protected override void OnStart()
        {
            currentChildIndex = 0;
        }

        protected override State OnUpdate()
        {
            if (children == null)
            {
                return State.Failure;
            }

            switch (children[currentChildIndex].Update())
            {
                case State.Running:
                    return State.Running;
                case State.Success:
                    currentChildIndex++;
                    break;
                case State.Failure:
                    return State.Failure;
            }

            return currentChildIndex == children.Count ? State.Success : State.Running;
        }
    }
}
