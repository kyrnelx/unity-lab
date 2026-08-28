using UnityEngine;

namespace Kyrnel
{
    public class PlayerWalkState : State
    {
        private Player _player;

        public PlayerWalkState(Player player)
        {
            _player = player;
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetKeyUp(KeyCode.Space))
            {
                _player.StateMachine?.ChangeState(_player.IdleState);
            }
        }
    }
}