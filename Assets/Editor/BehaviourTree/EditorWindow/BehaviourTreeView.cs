using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.TypeCache;
using TypeCache = UnityEditor.TypeCache;

namespace BehaviourTreeDesigner
{
    public class BehaviourTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

        public Action<NodeView> OnNodeSelected;
        public BehaviourTree tree;

        private const string AssetPath = "Assets/Editor/BehaviourTree/EditorWindow/BehaviourTreeEditor.uss";
        private const string NodeFilterName = "Node";

        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetPath);
            styleSheets.Add(styleSheet);

            Undo.ClearAll();
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        internal void PopulateView(BehaviourTree tree)
        {
            this.tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (tree.nodes == null)
            {
                tree.nodes = new List<Node>();
            }

            if (tree.rootNode == null)
            {
                tree.rootNode = CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            tree.nodes.ForEach(n => CreateNodeView(n));

            tree.nodes.ForEach(n =>
            {
                var children = tree.GetChildren(n);
                children.ForEach(c =>
                {
                    NodeView parentView = FindNodeView(n);
                    NodeView childView = FindNodeView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node)
                .ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(elem =>
            {
                if (elem is NodeView nodeView)
                {
                    DeleteNode(nodeView.node);
                }

                if (elem is Edge edge)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    RemoveChild(parentView.node, childView.node);
                }
            });

            graphViewChange.edgesToCreate?.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                AddChild(parentView.node, childView.node);
            });

            nodes.OfType<NodeView>().ToList().ForEach(view => view.SortChildren());

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            TypeCollection[] nodeTypes = new[]
            {
                GetTypesDerivedFrom<DecoratorNode>(),
                GetTypesDerivedFrom<CompositeNode>(),
                GetTypesDerivedFrom<ActionNode>(),
                GetTypesDerivedFrom<ConditionalNode>()
            };

            CreateMenuFromNodeTypes(nodeTypes, evt);
        }

        private void CreateMenuFromNodeTypes(TypeCollection[] nodeTypes, ContextualMenuPopulateEvent evt)
        {
            foreach (Type type in nodeTypes.SelectMany(types => types))
            {
                string baseTypeName = type.BaseType.Name.Replace(NodeFilterName, string.Empty);
                string nodeName = type.Name.Replace(NodeFilterName, string.Empty);

                string menuText = $"{baseTypeName}/{nodeName}";
                evt.menu.AppendAction(menuText, CreateNodeAction(type));
            }
        }

        private Action<DropdownMenuAction> CreateNodeAction(Type type)
        {
            return (action) =>
            {
                Node node = CreateNode(type);
                CreateNodeView(node);
            };
        }

        #region Node: Draw Functions

        public Node CreateNode(System.Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.guid = GUID.Generate().ToString();

            tree.BindNode(node);

            string nodeName = type.Name.Replace("Node", "");
            node.name = nodeName;
            
            Undo.RecordObject(tree, "Behaviour Tree (CreateNode)");
            tree.nodes.Add(node);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, tree);
            }

            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

            AssetDatabase.SaveAssets();
            return node;
        }

        public void CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(tree, "Behaviour Tree (DeleteNode)");
            tree.nodes.Remove(node);

            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        #endregion

        #region Edge: Draw Functions

        public void AddChild(Node parent, Node child)
        {
            if (parent is RootNode root)
            {
                Undo.RecordObject(root, "Behaviour Tree (AddChild)");
                root.child = child;
                EditorUtility.SetDirty(root);
            }
            
            if (parent is DecoratorNode decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }
            
            if (parent is CompositeNode composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            if (parent is RootNode root)
            {
                Undo.RecordObject(root, "Behaviour Tree (RemoveChild)");
                root.child = null;
                EditorUtility.SetDirty(root);
            }

            if (parent is DecoratorNode decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }

            if (parent is CompositeNode composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
            }
        }

        #endregion

        #region Visualization

        public void UpdateNodeState()
        {
            foreach (NodeView view in nodes)
            {
                view.UpdateState();
            }
        }

        #endregion
    }
}
