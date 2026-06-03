using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateTree
{
    public class StateTree: IStateTree
    {
        private readonly Dictionary<Type, IStateTreeNode> _states = new ();
        private Dictionary<Type, Task> _tasks = new ();
        private Dictionary<Type, Task> _tasks2 = new ();
        private IStateTreeNode _activeState;
        private IStateTreeNode _changingState;
        private readonly List<IStateTreeNode> _stateStack = new ();

        public IStateTreeNode ActiveState => _activeState;

        public IStateTreeBlackboard Blackboard { get; } = new StateTreeBlackboard();

        public void AddState<TState>(IStateTreeNode parent) where TState : IStateTreeNode, new()
        {
            if (parent != null)
            {
                if (!_states.TryGetValue(parent.GetType(), out var parentState))
                {
                    throw new Exception($"[StateTree] RegisterState parent {parent.GetType()} is not exist.");
                }
                if (parent != parentState)
                {
                    throw new Exception($"[StateTree] RegisterState parent {parent.GetType()} is not match");
                }
            }
            var state = new TState();
            if (!_states.TryAdd(typeof(TState), state))
            {
                throw new Exception($"[StateTree] RegisterState {typeof(TState)} is already exist.");
            }
            state.OnCreate(this, parent);
        }

        public TState GetState<TState>() where TState : IStateTreeNode
        {
            if (_states.TryGetValue(typeof(TState), out var state))
            {
                return (TState)state;
            }
            return default;
        }

        public async Task ChangeState<TState>() where TState : IStateTreeNode
        {
            if (_changingState != null)
            {
                return;
            }
            if (!_states.TryGetValue(typeof(TState), out var state))
            {
                return;
            }
            await _ChangeState(state);
        }

        public async Task PushState<TState>() where TState : IStateTreeNode
        {
            if (_changingState != null)
            {
                return;
            }
            if (!_states.TryGetValue(typeof(TState), out var state))
            {
                return;
            }
            if (state != _activeState)
            {
                _stateStack.Add(_activeState);
            }
            await _ChangeState(state);
        }

        public async Task PopState()
        {
            if (_changingState != null)
            {
                return;
            }
            while (_stateStack.Count > 0 && _activeState == _stateStack[^1])
            {
                _stateStack.RemoveAt(_stateStack.Count - 1);
            }
            if (_stateStack.Count <= 0)
            {
                return;
            }
            var state = _stateStack[^1];
            _stateStack.RemoveAt(_stateStack.Count - 1);
            await _ChangeState(state);
        }

        public void ClearStateStack()
        {
            _stateStack.Clear();
        }

        private async Task<bool> _ChangeState(IStateTreeNode state)
        {
            if (_changingState != null)
            {
                return false;
            }
            if (state == _activeState)
            {
                return false;
            }
            List<IStateTreeNode> enterPath = new List<IStateTreeNode>();
            Dictionary<IStateTreeNode, int> visited = new Dictionary<IStateTreeNode, int>();
            var tempState = state;
            while (tempState != null)
            {
                visited.Add(tempState, enterPath.Count);
                enterPath.Add(tempState);
                tempState = tempState.Parent;
            }
            List<IStateTreeNode> exitPath = new List<IStateTreeNode>();
            IStateTreeNode crossState = null;
            tempState = _activeState;
            int enterStartIndex = enterPath.Count - 1;
            while (tempState != null)
            {
                if (visited.TryGetValue(tempState, out var value))
                {
                    crossState = tempState;
                    enterStartIndex = value - 1;
                    break;
                }
                exitPath.Add(tempState);
                tempState = tempState.Parent;
            }
            _changingState = state;
            var oldState = _activeState;
            
            foreach (var stateTreeNode in exitPath)
            {
                await stateTreeNode.OnPrepareExit(state, oldState);
            }
            if (crossState != null)
            {
                await crossState.OnPrepareSwitch(state, oldState);
            }
            for (int i = enterStartIndex; i >=0; i--)
            {
                await enterPath[i].OnPrepareEnter(state, oldState);
            }

            foreach (var stateTreeNode in exitPath)
            {
                await stateTreeNode.OnExit(state, oldState);
            }
            if (crossState != null)
            {
                if (enterStartIndex < 0)
                {
                    _changingState = null;
                    _activeState = state;
                }
                await crossState.OnSwitch(state, oldState);
            }
            for (int i = enterStartIndex; i >=0; i--)
            {
                if (i == 0)
                {
                    _changingState = null;
                    _activeState = state;
                }
                await enterPath[i].OnEnter(state, oldState);
            }

            return true;
        }
        
        private List<IStateTreeNode> GetEnterPath(IStateTreeNode state)
        {
            List<IStateTreeNode> enterPath = new List<IStateTreeNode>();
            var tempState = state;
            while (tempState != null)
            {
                enterPath.Add(tempState);
                tempState = tempState.Parent;
            }
            return enterPath;
        }

        public void OnUpdate(double delta)
        {
            List<IStateTreeNode> enterPath = GetEnterPath(_activeState);
            for (var i = enterPath.Count - 1; i >= 0; i--)
            {
                var state = enterPath[i];
                var t = state.GetType();
                if (_tasks.TryGetValue(t, out var task) && !task.IsCompleted)
                {
                    continue;
                }
                _tasks[t] = enterPath[i].OnUpdate(delta);
            }
            foreach (var kv in _tasks)
            {
                if (!kv.Value.IsCompleted)
                {
                    _tasks2.Add(kv.Key, kv.Value);
                }
            }
            _tasks.Clear();
            (_tasks, _tasks2) = (_tasks2, _tasks);
        }

        public async Task DoAction<T>(T action)
        {
            var state = _activeState;
            state = state?.Parent;
            while (state != null && await state.OnAction(action))
            {
                state = state.Parent;
            }
        }

        public async Task DoEvent<T>(T e)
        {
            List<IStateTreeNode> enterPath = GetEnterPath(_activeState);
            for (var i = enterPath.Count - 1; i >= 0; i--)
            {
                if (!await enterPath[i].OnEvent(e))
                {
                    break;
                }
            }
        }
    }
}