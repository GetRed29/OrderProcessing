using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.States
{
    public class PendingState : IOrderState
    {
        public string Name => "Pending";
        public void Pay(Order o)
        {
            o.AddHistory(Name, "Confirmed");
            
            o.CurrentState = new ConfirmedState();
        }
        public void Process(Order o) => throw new InvalidOrderTransitionException(Name, "Process");
        public void Ship(Order o) => throw new InvalidOrderTransitionException(Name, "Ship");
        public void Deliver(Order o) => throw new InvalidOrderTransitionException(Name, "Deliver");
        public void Cancel(Order o)
        {
            o.AddHistory(Name, "Cancelled");

            o.CurrentState = new CancelledState();
        }
    }
}
