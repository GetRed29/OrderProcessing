using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.States
{
    public class ConfirmedState : IOrderState
    {
        public string Name => "Confirmed";
        public void Pay(Order o) => throw new InvalidOrderTransitionException(Name, "Pay");
        public void Process(Order o)
        {
            o.AddHistory(Name, "Processing");

            o.CurrentState = new ProcessingState();
        }
        public void Ship(Order o) => throw new InvalidOrderTransitionException(Name, "Ship");
        public void Deliver(Order o) => throw new InvalidOrderTransitionException(Name, "Deliver");
        public void Cancel(Order o)
        {
            o.AddHistory(Name, "Cancelled");

            o.CurrentState = new CancelledState();
        }
    }
}
