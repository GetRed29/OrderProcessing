using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.States
{
    public class DeliveredState : IOrderState
    {
        public string Name => "Delivered";
        public void Pay(Order o) => throw new InvalidOrderTransitionException(Name, "Pay");
        public void Process(Order o) => throw new InvalidOrderTransitionException(Name, "Process");
        public void Ship(Order o) => throw new InvalidOrderTransitionException(Name, "Ship");
        public void Deliver(Order o) => throw new InvalidOrderTransitionException(Name, "Deliver");
        public void Cancel(Order o) => throw new InvalidOrderTransitionException(Name, "Cancel");
    }
}
