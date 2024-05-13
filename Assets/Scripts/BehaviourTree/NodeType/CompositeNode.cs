using System.Collections.Generic;

namespace BehaviourTreeDesigner
{
    public abstract class CompositeNode : Node
    {
        public List<Node> children = new();

        public override Node Clone()
        {
            CompositeNode node = Instantiate(this);
            node.children = children.ConvertAll(childNode => childNode.Clone());
            return node;
        }
    }
}
