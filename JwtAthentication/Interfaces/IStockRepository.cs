using SecondApp.Dtos.Stock;
using SecondApp.Models;

namespace SecondApp.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync();
        Task<Stock?> GetByIdAsync(int id);
        Task<Stock> CreateStockAsync(Stock stockModel );
        Task<Stock> UpdateStockAsync(int id, UpdateStockDto stockModel);
    }
}
