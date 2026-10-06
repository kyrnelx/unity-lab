using UnityEngine;

namespace Kyrnel
{
    public class Player : MonoBehaviour
    {
        public StateMachine StateMachine { get; set; }
        public PlayerIdleState IdleState { get; set; }
        public PlayerWalkState WalkState { get; set; }
        
        public MoveCommand MoveCommand { get; set; }

        private void Awake()
        {
            IdleState = new PlayerIdleState(this);
            WalkState = new PlayerWalkState(this);
            StateMachine = new StateMachine(IdleState);

            MoveCommand = new MoveCommand(this, new Vector3(0, 1, 0));
        }

        private void Update()
        {
            StateMachine?.Update();
            if (Input.GetKeyDown(KeyCode.E))
                CommandManager.Instance.Execute(MoveCommand);
            if (Input.GetKeyDown(KeyCode.U))
                CommandManager.Instance.Undo();
            if (Input.GetKeyDown(KeyCode.R))
                CommandManager.Instance.Redo();
        }

        public void Move(Vector3 offset)
        {
            transform.position += offset;
        }
    }
}