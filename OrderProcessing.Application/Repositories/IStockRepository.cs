namespace OrderProcessing.Application.Repositories
{
    public interface IStockRepository
    {
        Task<int> GetStockAsync(Guid productId);
        Task ReduceStockAsync(Guid productId, int quantity);
    }
}
