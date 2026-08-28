using UnityEngine;

namespace Kyrnel
{
    public class MoveCommand : ICommand
    {
        private Player  _player;
        private Vector3 _offset;

        public MoveCommand(Player player, Vector3 offset)
        {
            _player = player;
            _offset = offset;
        }

        public void Execute()
        {
            _player.Move(_offset);
        }

        public void Undo()
        {
            _player.Move(-_offset);
        }
    }
}