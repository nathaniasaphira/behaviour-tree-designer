using UnityEngine;

namespace BehaviourTreeDesigner
{
    public class BehaviourTreeBuilder : MonoBehaviour
    {
        [SerializeField]
        private GameObject target;

        public BehaviourTree tree;

        private void Awake()
        {
            tree = tree.Clone();
            tree.Bind(this.gameObject, target);
        }

        private void Update()
        {
            tree.Update();
        }
    }
}
