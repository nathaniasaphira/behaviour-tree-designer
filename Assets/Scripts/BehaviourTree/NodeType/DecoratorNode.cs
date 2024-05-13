using UnityEngine;

namespace BehaviourTreeDesigner
{
    public abstract class DecoratorNode : Node
    {
        public Node child;

        public override Node Clone()
        {
            DecoratorNode node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}
