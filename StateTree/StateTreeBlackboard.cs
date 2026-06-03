using System.Collections.Generic;

namespace StateTree
{
    public class StateTreeBlackboard : IStateTreeBlackboard
    {
        private readonly Dictionary<object, object> _data = new ();
        private readonly IStateTreeBlackboard _parent;

        public StateTreeBlackboard(IStateTreeBlackboard parent = null)
        {
            _parent = parent;
        }
        
        public T Get<T>(object key)
        {
            if (_data.TryGetValue(key, out var v) && v is T value)
            {
                return value;
            }

            if (_parent != null)
            {
                return _parent.Get<T>(key);
            }

            return default;
        }
        
        public void Set<T>(object key, T value)
        {
            _data[key] = value;
        }

        public void Remove(object key)
        {
            _data.Remove(key);
        }
    }
}