namespace BehaviourTreeDesigner
{
    public class ReturnSuccessNode : DecoratorNode
    {
        protected override State OnUpdate()
        {
            child.Update();
            return State.Success;
        }
    }
}
