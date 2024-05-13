using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BehaviourTreeDesigner
{
    [CustomEditor(typeof(BoolComparisonNode))]
    public class BoolComparisonNodeInspector : Editor
    {
        private readonly BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BoolComparisonNode boolComparator = (BoolComparisonNode)target;
            
            if (boolComparator.SelectedTargetIndex < 0 )
            {
                boolComparator.SelectedTargetIndex = 0;
            }

            string[] propertyNames = GetPropertyNames(typeof(GameObject));
            boolComparator.SelectedTargetIndex = EditorGUILayout.Popup("Target Property", boolComparator.SelectedTargetIndex, propertyNames);
        }

        private string[] GetPropertyNames(System.Type type)
        {
            PropertyInfo[] properties = type.GetProperties(bindingFlags);

            IEnumerable<PropertyInfo> boolProperties = properties.Where(p => p.PropertyType == typeof(bool));

            return boolProperties.Select(p => p.Name).ToArray();
        }
    }
}