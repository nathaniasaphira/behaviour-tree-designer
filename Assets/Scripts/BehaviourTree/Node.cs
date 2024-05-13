using UnityEngine;

namespace BehaviourTreeDesigner
{
    public abstract class Node : ScriptableObject
    {   
        public enum State
        {
            Running,
            Success,
            Failure
        }

        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public SharedData sharedData;

        public State CurrentState = State.Running;

        private bool isStarted = false;

        public State Update()
        {
            if (!isStarted)
            {
                OnStart();
                isStarted = true;
            }

            CurrentState = OnUpdate();

            if (CurrentState is not State.Running)
            {
                OnFinish();
                isStarted = false;
            }

            return CurrentState;
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        protected virtual void OnStart() { }
        
        protected virtual void OnFinish() { }

        protected abstract State OnUpdate();
    }
}