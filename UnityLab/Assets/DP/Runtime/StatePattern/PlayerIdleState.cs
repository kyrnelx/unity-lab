using UnityEngine;

namespace Kyrnel
{
    public class PlayerIdleState : State
    {
        private Player _player;

        public PlayerIdleState(Player player)
        {
            _player = player;
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _player.StateMachine?.ChangeState(_player.WalkState);
            }
        }
    }
}