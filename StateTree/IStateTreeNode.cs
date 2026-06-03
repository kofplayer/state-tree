using System.Threading.Tasks;

namespace StateTree
{
    public interface IStateTreeNode
    {
        IStateTree Tree { get; }
        IStateTreeNode Parent { get; }
        IStateTreeBlackboard Blackboard { get; }

        void OnCreate(IStateTree stateTree, IStateTreeNode parent);
        Task OnPrepareEnter(IStateTreeNode newState, IStateTreeNode oldState);
        Task OnPrepareExit(IStateTreeNode newState, IStateTreeNode oldState);
        Task OnPrepareSwitch(IStateTreeNode newState, IStateTreeNode oldState);
        Task OnEnter(IStateTreeNode newState, IStateTreeNode oldState);
        Task OnExit(IStateTreeNode newState, IStateTreeNode oldState);
        Task OnSwitch(IStateTreeNode newState, IStateTreeNode oldState);
        Task OnUpdate(double delta);
        Task<bool> OnAction<T>(T action);
        Task<bool> OnEvent<T>(T e);
    }
}