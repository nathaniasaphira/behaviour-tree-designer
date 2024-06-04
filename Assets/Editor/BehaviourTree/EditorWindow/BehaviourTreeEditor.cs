using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTreeDesigner
{
    public class BehaviourTreeEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        private const string WindowTitle = "Behavior Tree Editor";
        private const string NewBTButtonName = "NewBehaviourButton";
        private const string BTAssetPath = "Assets/Prefabs/Behaviour/";

        private BehaviourTreeView treeView;
        private InspectorView inspectorView;
        private TextField newFileTextField;
        private Button newBTButton;

        [MenuItem("BehaviorTree/Behaviour Tree Editor")]
        public static void OpenEditorWindow()
        {
            BehaviourTreeEditor window = GetWindow<BehaviourTreeEditor>();
            window.titleContent = new GUIContent(WindowTitle);
        }

        [OnOpenAsset()]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            OpenEditorWindow();
            return true;
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        // visualization
        private void OnInspectorUpdate()
        {
            treeView?.UpdateNodeState();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            m_VisualTreeAsset.CloneTree(root);

            treeView = root.Q<BehaviourTreeView>();
            treeView.focusable = true;

            inspectorView = root.Q<InspectorView>();

            newFileTextField = root.Query<TextField>().First();

            newBTButton = root.Q<ToolbarButton>(NewBTButtonName);
            newBTButton.clicked += OnNewButtonClicked;

            treeView.OnNodeSelected = OnNodeSelectionChanged;
            OnSelectionChange();
        }

        private void OnNewButtonClicked()
        {
            BehaviourTree file = CreateInstance<BehaviourTree>();
            
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(BTAssetPath + newFileTextField.text + ".asset");
            AssetDatabase.CreateAsset(file, assetPath);

            file = (BehaviourTree)AssetDatabase.LoadAssetAtPath(assetPath, typeof(BehaviourTree));
            treeView.PopulateView(file);
            AssetDatabase.SaveAssets();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void OnSelectionChange()
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (!tree)
            {
                if (Selection.activeGameObject)
                {
                    BehaviourTreeBuilder builder = Selection.activeGameObject.GetComponent<BehaviourTreeBuilder>();
                    if (builder)
                    {
                        tree = builder.tree;
                    }
                }
            }

            if (Application.isPlaying)
            {
                if (tree)
                {
                    treeView.PopulateView(tree);
                }
            }
            else
            {
                if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                {
                    treeView.PopulateView(tree);
                }
            }
        }

        private void OnNodeSelectionChanged(NodeView nodeView)
        {
            inspectorView.UpdateSelection(nodeView);
        }
    }
}
