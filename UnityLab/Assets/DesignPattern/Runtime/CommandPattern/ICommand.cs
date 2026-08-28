namespace Kyrnel
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}