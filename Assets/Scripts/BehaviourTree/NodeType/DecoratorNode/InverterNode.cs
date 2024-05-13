namespace BehaviourTreeDesigner
{
    public class InverterNode : DecoratorNode
    {
        protected override State OnUpdate()
        {
            if (child == null)
            {
                UnityEngine.Debug.LogWarning($"[Inverter] Child is null");
                return State.Failure;
            }

            switch (child.Update())
            {
                case State.Running:
                    break;
                case State.Success:
                    return State.Failure;
                case State.Failure:
                    return State.Success;
            }

            return State.Running;
        }
    }
}