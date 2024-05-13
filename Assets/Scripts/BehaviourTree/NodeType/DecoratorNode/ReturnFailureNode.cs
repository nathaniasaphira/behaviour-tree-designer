namespace BehaviourTreeDesigner
{
    public class ReturnFailureNode : DecoratorNode
    {
        protected override State OnUpdate()
        {
            child.Update();
            return State.Failure;
        }
    }
}
