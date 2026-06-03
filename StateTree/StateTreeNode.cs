using System;
using System.Threading.Tasks;

namespace StateTree
{
    public class StateTreeNode: IStateTreeNode
    {
        private IStateTree _stateTree;
        private IStateTreeNode _parent;
        protected IStateTreeBlackboard CustomBlackboard;

        public IStateTreeBlackboard Blackboard => CustomBlackboard ?? _parent?.Blackboard ?? _stateTree.Blackboard;
        
        public IStateTree Tree => _stateTree;
        public IStateTreeNode Parent => _parent;

        public virtual void OnCreate(IStateTree stateTree, IStateTreeNode parent)
        {
            _parent = parent;
            _stateTree = stateTree;
            Console.WriteLine($"[StateTree] {GetType().Name}.OnCreate parent is [{parent?.GetType().Name}]");
        }

        public Task OnPrepareEnter(IStateTreeNode newState, IStateTreeNode oldState)
        {
            Console.WriteLine($"[StateTree] {GetType().Name}.OnPrepareEnter [{oldState?.GetType().Name} -> {newState?.GetType().Name}]");
            return Task.CompletedTask;
        }

        public Task OnPrepareExit(IStateTreeNode newState, IStateTreeNode oldState)
        {
            Console.WriteLine($"[StateTree] {GetType().Name}.OnPrepareExit [{oldState?.GetType().Name} -> {newState?.GetType().Name}]");
            return Task.CompletedTask;
        }

        public Task OnPrepareSwitch(IStateTreeNode newState, IStateTreeNode oldState)
        {
            Console.WriteLine($"[StateTree] {GetType().Name}.OnPrepareSwitch [{oldState?.GetType().Name} -> {newState?.GetType().Name}]");
            return Task.CompletedTask;
        }

        public virtual Task OnEnter(IStateTreeNode newState, IStateTreeNode oldState)
        {
            Console.WriteLine($"[StateTree] {GetType().Name}.OnEnter [{oldState?.GetType().Name} -> {newState?.GetType().Name}]");
            return Task.CompletedTask;
        }

        public virtual Task OnExit(IStateTreeNode newState, IStateTreeNode oldState)
        {
            Console.WriteLine($"[StateTree] {GetType().Name}.OnExit [{oldState?.GetType().Name} -> {newState?.GetType().Name}]");
            return Task.CompletedTask;
        }

        public virtual Task OnSwitch(IStateTreeNode newState, IStateTreeNode oldState)
        {
            Console.WriteLine($"[StateTree] {GetType().Name}.OnSwitch [{oldState?.GetType().Name} -> {newState?.GetType().Name}]");
            return Task.CompletedTask;
        }

        public virtual Task OnUpdate(double delta)
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> OnAction<T>(T action)
        {
            return Task.FromResult(true);
        }

        public virtual Task<bool> OnEvent<T>(T e)
        {
            return Task.FromResult(true);
        }

        protected void AddState<TState>() where TState : IStateTreeNode, new()
        {
            _stateTree.AddState<TState>(this);
        }
    }
}