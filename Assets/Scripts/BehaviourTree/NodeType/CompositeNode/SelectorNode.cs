namespace BehaviourTreeDesigner
{
    public class SelectorNode : CompositeNode
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
                    return State.Success;
                case State.Failure:
                    currentChildIndex++;
                    break;
            }

            return currentChildIndex == children.Count ? State.Failure : State.Running;
        }
    }
}
