using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.States
{
    public class ShippedState : IOrderState
    {
        public string Name => "Shipped";
        public void Pay(Order o) => throw new InvalidOrderTransitionException(Name, "Pay");
        public void Process(Order o) => throw new InvalidOrderTransitionException(Name, "Process");
        public void Ship(Order o) => throw new InvalidOrderTransitionException(Name, "Ship");
        public void Deliver(Order o)
        {
            o.AddHistory(Name, "Delivered");
            
            o.CurrentState = new DeliveredState();
        }
        public void Cancel(Order o)
        {
            o.AddHistory(Name, "Cancelled");
            
            o.CurrentState = new CancelledState();
        }
    }
}