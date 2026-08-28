using UnityEngine;

namespace Kyrnel
{
    public abstract class State : IState
    {
        public virtual void Enter()
        {
            Debug.Log($"Entering {GetType().Name} State");
        }

        public virtual void Update()
        {
            Debug.Log($"Updating {GetType().Name} State");
        }

        public virtual void Exit()
        {
            Debug.Log($"Exit {GetType().Name} State");
        }
    }
}