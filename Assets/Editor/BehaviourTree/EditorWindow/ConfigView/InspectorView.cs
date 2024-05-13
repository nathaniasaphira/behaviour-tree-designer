using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviourTreeDesigner
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        private Editor editor;

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();            
            UnityEngine.Object.DestroyImmediate(editor);

            editor = Editor.CreateEditor(nodeView.node);
            
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}
