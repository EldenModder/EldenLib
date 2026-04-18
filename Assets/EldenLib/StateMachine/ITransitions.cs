namespace EldenLib.StateMachine
{
    public interface ITransitions 
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}
