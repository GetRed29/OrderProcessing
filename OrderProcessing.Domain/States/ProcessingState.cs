using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.States
{
    public class ProcessingState : IOrderState
    {
        public string Name => "Processing";
        public void Pay(Order o) => throw new InvalidOrderTransitionException(Name, "Pay");
        public void Process(Order o) => throw new InvalidOrderTransitionException(Name, "Process");
        public void Ship(Order o)
        {
            o.AddHistory(Name, "Shipped");
            
            o.CurrentState = new ShippedState();
        }
        public void Deliver(Order o) => throw new InvalidOrderTransitionException(Name, "Deliver");
        public void Cancel(Order o)
        {
            o.AddHistory(Name, "Cancelled");

            o.CurrentState = new CancelledState();
        }
    }
}
