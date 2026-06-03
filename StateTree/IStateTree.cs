using System.Threading.Tasks;

namespace StateTree
{
    public interface IStateTree
    {
        IStateTreeBlackboard Blackboard { get; }
        
        IStateTreeNode ActiveState { get; }
        
        void AddState<TState>(IStateTreeNode parent) where TState : IStateTreeNode, new();

        TState GetState<TState>() where TState : IStateTreeNode;

        Task ChangeState<TState>() where TState : IStateTreeNode;
        
        Task PushState<TState>() where TState : IStateTreeNode;
        
        Task PopState();
        
        void ClearStateStack();

        void OnUpdate(double delta);

        Task DoAction<T>(T action);

        Task DoEvent<T>(T e);
    }
}