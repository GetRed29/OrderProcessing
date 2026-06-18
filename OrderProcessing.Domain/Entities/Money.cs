namespace OrderProcessing.Domain.Entities
{
    public record Money(decimal Amount, string Currency)
    {
        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies.");
            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator *(Money money, int multiplier)
        {
            return new Money(money.Amount * multiplier, money.Currency);
        }
    }
}
