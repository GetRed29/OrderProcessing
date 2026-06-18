namespace OrderProcessing.Domain.States
{
    public class InvalidOrderTransitionException : InvalidOperationException
    {
        public InvalidOrderTransitionException(string from, string action)
            : base($"Cannot perform '{action}' action from '{from}' state.") { }
    }
}
