namespace StateTree
{
    public interface IStateTreeBlackboard
    {
        T Get<T>(object key);
        void Set<T>(object key, T value);
        void Remove(object key);
    }
}