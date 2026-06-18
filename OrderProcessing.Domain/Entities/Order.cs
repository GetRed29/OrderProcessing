using OrderProcessing.Domain.States;

namespace OrderProcessing.Domain.Entities
{
    public class Order
    {
        public OrderId Id { get; private set; }
        public Customer Customer { get; private set; }
        public Address ShippingAddress { get; private set; }
        public IReadOnlyCollection<OrderItem> Items {  get; private set; }
        public Money TotalAmount { get; private set; }
        public IOrderState CurrentState { get; internal set; } = new PendingState();
        public string Status => CurrentState.Name;
        public record OrderHistoryEntry(string FromState, string ToState, DateTime At);
        private readonly List<OrderHistoryEntry> _history = new();
        public IReadOnlyCollection<OrderHistoryEntry> History => _history.AsReadOnly();

        public Order(OrderId id, Customer customer, Address shippingAddress, List<OrderItem> items, Money totalAmount)
        {
            Id = id;
            Customer = customer;
            ShippingAddress = shippingAddress;
            Items = items;
            TotalAmount = totalAmount;
        }

        internal void AddHistory(string fromState, string toState)
        {
            _history.Add(new OrderHistoryEntry(fromState, toState, DateTime.UtcNow));
        }

        public void Pay() => CurrentState.Pay(this);
        public void Process() => CurrentState.Process(this);
        public void Ship() => CurrentState.Ship(this);
        public void Deliver() => CurrentState.Deliver(this);
        public void Cancel() => CurrentState.Cancel(this);
    }
}
