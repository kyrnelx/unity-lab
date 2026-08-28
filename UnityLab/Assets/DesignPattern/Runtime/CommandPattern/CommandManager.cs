using System.Collections.Generic;
using UnityEngine;

namespace Kyrnel
{
    public class CommandManager : MonoBehaviour
    {
        public static CommandManager Instance { get; private set; }

        private Stack<ICommand> _undoStack = new Stack<ICommand>();
        private Stack<ICommand> _redoStack = new Stack<ICommand>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void Execute(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
            Debug.Log($"执行命令{nameof(command)}");
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                ICommand command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
                Debug.Log($"撤销命令{nameof(command)}");
            }
            else
            {
                Debug.Log("没有可以撤销的操作了！");
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                ICommand command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
                Debug.Log($"恢复命令{nameof(command)}");
            }
            else
            {
                Debug.Log("没有可以重做（恢复）的操作了！");
            }
        }
    }
}