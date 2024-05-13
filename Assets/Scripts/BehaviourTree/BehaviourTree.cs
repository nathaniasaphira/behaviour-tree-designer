using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeDesigner
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        public Node rootNode;
        public Node.State treeState = Node.State.Running;
        public List<Node> nodes;
        public SharedData sharedData;

        public Node.State Update()
        {
            if (rootNode.CurrentState is Node.State.Running)
            {
                return rootNode.Update();
            }

            return treeState;
        }

        public BehaviourTree Clone()
        {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();

            Traverse(tree.rootNode, tree.nodes.Add);

            return tree;
        }

        public void Bind(GameObject thisObject = null, GameObject targetObject = null)
        {
            sharedData.ThisObject ??= thisObject;
            sharedData.TargetObject ??= targetObject;

            Traverse(rootNode, node =>
            {
                node.sharedData = sharedData;
            });
        }

        public void BindNode(Node node)
        {
            node.sharedData = sharedData;
        }

        public void Traverse(Node node, System.Action<Node> visitor)
        {
            if (node != null)
            {
                visitor.Invoke(node);
                var children = GetChildren(node);
                children.ForEach(n => Traverse(n, visitor));
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new();

            if (parent is RootNode root && root.child != null)
            {
                children.Add(root.child);
            }

            if (parent is DecoratorNode decorator && decorator.child != null)
            {
                children.Add(decorator.child);
            }

            if (parent is CompositeNode composite)
            {
                return composite.children;
            }

            return children;
        }
    }
}