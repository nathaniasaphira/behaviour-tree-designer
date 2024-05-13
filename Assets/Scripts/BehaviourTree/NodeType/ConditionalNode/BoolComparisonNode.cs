using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BehaviourTreeDesigner
{
    public class BoolComparisonNode : ConditionalNode
    {
        [Header("Bool Comparison Configuration")]
        [SerializeField] private bool compareValue;

        [HideInInspector] public int SelectedTargetIndex;

        private readonly BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

        protected override void OnStart()
        {
            PropertyInfo targetPropertyInfo = GetSelectedProperty(sharedData.ThisObject.GetType());
            var targetVariable = targetPropertyInfo.GetValue(sharedData.ThisObject);

            condition = (bool)targetVariable;
        }

        protected override State OnUpdate()
        {
            return condition == compareValue ? State.Success : State.Failure;
        }

        private PropertyInfo GetSelectedProperty(System.Type type)
        {
            PropertyInfo[] properties = type.GetProperties(bindingFlags);
            IEnumerable<PropertyInfo> boolProperties = properties.Where(p => p.PropertyType == typeof(bool));
            properties = boolProperties.ToArray();

            return properties[SelectedTargetIndex];
        }
    }
}
