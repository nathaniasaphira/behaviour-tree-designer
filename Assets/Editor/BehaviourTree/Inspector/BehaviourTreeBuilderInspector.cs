using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeDesigner
{
    [CustomEditor(typeof(BehaviourTreeBuilder))]
    public class BehaviourTreeBuilderInspector : Editor
    {
        private const float SpaceSize = 3f;
        private const string BehaviourTreeField = "Behaviour Tree";

        private Dictionary<System.Type, string> objectFields = new()
        {
            { typeof(GameObject), "This Object" },
            { typeof(GameObject), "Target Object" }
        };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BehaviourTreeBuilder treeBuilder = (BehaviourTreeBuilder)target;

            EditorGUILayout.Space(SpaceSize);

            if (treeBuilder.tree != null)
            {
                EditorGUILayout.LabelField("Shared Data", EditorStyles.boldLabel);
                DrawSharedDataFields(treeBuilder.tree.sharedData);
            }
        }

        private void DrawSharedDataFields(SharedData sharedData)
        {
            sharedData.ThisObject = (GameObject)EditorGUILayout.ObjectField(
                objectFields[typeof(GameObject)], sharedData.ThisObject, typeof(GameObject), true);

            sharedData.TargetObject = (GameObject)EditorGUILayout.ObjectField(
                objectFields[typeof(GameObject)], sharedData.TargetObject, typeof(GameObject), true);
        }
    }
}
