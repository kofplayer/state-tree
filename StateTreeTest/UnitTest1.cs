using StateTree;

namespace StateTreeTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }
    
    class State1 : StateTree.StateTreeNode
    {
        public override void OnCreate(IStateTree stateTree, IStateTreeNode parent)
        {
            base.OnCreate(stateTree, parent);
            AddState<State2>();
            AddState<State3>();
        }
    }
    
    class State2 : StateTree.StateTreeNode
    {
        public override void OnCreate(IStateTree stateTree, IStateTreeNode parent)
        {
            base.OnCreate(stateTree, parent);
        }
    }
    
    class State3 : StateTree.StateTreeNode
    {
        public override void OnCreate(IStateTree stateTree, IStateTreeNode parent)
        {
            base.OnCreate(stateTree, parent);
            AddState<State4>();
            AddState<State5>();
        }
    }
    
    class State4 : StateTree.StateTreeNode
    {
        public override void OnCreate(IStateTree stateTree, IStateTreeNode parent)
        {
            base.OnCreate(stateTree, parent);
        }
    }
    
    class State5 : StateTree.StateTreeNode
    {
        public override void OnCreate(IStateTree stateTree, IStateTreeNode parent)
        {
            base.OnCreate(stateTree, parent);
        }
    }

    [Test]
    /*
     * State1
     *  - State2
     *  - State3
     *     - State4
     *     - State5
     */
    public async Task Test1()
    {
        var tree = new StateTree.StateTree();
        
        /*
            [StateTree] State1.OnCreate parent is []
            [StateTree] State2.OnCreate parent is [State1]
            [StateTree] State3.OnCreate parent is [State1]
            [StateTree] State4.OnCreate parent is [State3]
            [StateTree] State5.OnCreate parent is [State3]
         */
        tree.AddState<State1>(null);
        
        /*
            [StateTree] State1.OnPrepareEnter [ -> State2]
            [StateTree] State2.OnPrepareEnter [ -> State2]
            [StateTree] State1.OnEnter [ -> State2]
            [StateTree] State2.OnEnter [ -> State2]
         */
        await tree.ChangeState<State2>();
        
        /*
            [StateTree] State2.OnPrepareExit [State2 -> State4]
            [StateTree] State1.OnPrepareSwitch [State2 -> State4]
            [StateTree] State3.OnPrepareEnter [State2 -> State4]
            [StateTree] State4.OnPrepareEnter [State2 -> State4]
            [StateTree] State2.OnExit [State2 -> State4]
            [StateTree] State1.OnSwitch [State2 -> State4]
            [StateTree] State3.OnEnter [State2 -> State4]
            [StateTree] State4.OnEnter [State2 -> State4]
         */
        await tree.ChangeState<State4>();
        
        /*
            [StateTree] State4.OnPrepareExit [State4 -> State5]
            [StateTree] State3.OnPrepareSwitch [State4 -> State5]
            [StateTree] State5.OnPrepareEnter [State4 -> State5]
            [StateTree] State4.OnExit [State4 -> State5]
            [StateTree] State3.OnSwitch [State4 -> State5]
            [StateTree] State5.OnEnter [State4 -> State5]
         */
        await tree.PushState<State5>();
        
        /*
            [StateTree] State5.OnPrepareExit [State5 -> State4]
            [StateTree] State3.OnPrepareSwitch [State5 -> State4]
            [StateTree] State4.OnPrepareEnter [State5 -> State4]
            [StateTree] State5.OnExit [State5 -> State4]
            [StateTree] State3.OnSwitch [State5 -> State4]
            [StateTree] State4.OnEnter [State5 -> State4]
         */
        await tree.PopState();
        
        Assert.Pass();
    }
}