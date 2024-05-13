using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTreeDesigner
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> OnNodeSelected;

        public Node node;
        public Port input;
        public Port output;

        private readonly Dictionary<Node.State, string> stateClassList = new()
        {
            { Node.State.Running, "running" },
            { Node.State.Success, "success" },
            { Node.State.Failure, "failure" }
        };
        private Node.State previousNodeState;

        public NodeView(Node node) : base("Assets/Editor/BehaviourTree/EditorWindow/NodeView/NodeView.uxml")
        {
            this.node = node;
            this.title = node.name;
            this.viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;

            previousNodeState = node.CurrentState;

            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
        }

        private void SetupClasses()
        {
            // add node base class
            if (node is ConditionalNode)
            {
                AddToClassList("conditional");
            }
            else if (node is ActionNode)
            {
                AddToClassList("action");
            }
            else if (node is CompositeNode)
            {
                AddToClassList("composite");
            }
            else if (node is DecoratorNode)
            {
                AddToClassList("decorator");
            }
            else if (node is RootNode)
            {
                AddToClassList("root");
            }

            // add specific node class
            string nodeClass = node.name.ToLower().Replace("(clone)", "");
            AddToClassList(nodeClass);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            Undo.RecordObject(node, "Behavior Tree (Set Position)");

            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;

            EditorUtility.SetDirty(node);
        }

        public override void OnSelected()
        {
            base.OnSelected();

            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }

        #region Node Port: Draw Functions

        private void CreateInputPorts()
        {
            if (node is ConditionalNode || node is ActionNode || node is CompositeNode || node is DecoratorNode)
            {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }

            if (input != null)
            {
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts()
        {
            if (node is CompositeNode)
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            else if (node is DecoratorNode || node is RootNode)
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }

            if (output != null)
            {
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(output);
            }
        }

        #endregion

        #region Node Order: Sort functions

        public void SortChildren()
        {
            CompositeNode composite = node as CompositeNode;
            if (composite)
            {
                composite.children.Sort(SortByHorizontalPosition);
            }
        }

        private int SortByHorizontalPosition(Node left, Node right)
        {
            return left.position.x < right.position.x ? -1 : 1;
        }

        #endregion

        #region Visualization

        public void UpdateState()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (previousNodeState != node.CurrentState)
            {
                RemoveFromClassList(stateClassList[previousNodeState]);
                previousNodeState = node.CurrentState;
            }

            AddToClassList(stateClassList[node.CurrentState]);
        }

        #endregion
    }
}
