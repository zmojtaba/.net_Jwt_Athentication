using Microsoft.EntityFrameworkCore;
using SecondApp.Data;
using SecondApp.Dtos.Stock;
using SecondApp.Interfaces;
using SecondApp.Models;

namespace SecondApp.Repository
{
    public class StockRespository : IStockRepository
    {
        private readonly ApplicationDbContext _context;
        public StockRespository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateStockAsync(Stock stockModel)
        { 
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public Task<List<Stock>> GetAllAsync()
        {
            return _context.Stocks.Include( c => c.Comments).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            Stock? stock = await _context.Stocks.SingleOrDefaultAsync(x => x.Id == id);
            return stock;
        }


        public async Task<Stock> UpdateStockAsync(int id, UpdateStockDto stockModel)
        {
            Stock stock = await GetByIdAsync(id);

            if (stock == null)
            {
                return null;
            }

            stock.purchase = stockModel.purchase;   
            stock.divdend = stockModel.divdend; 
            stock.CompanyName = stockModel.CompanyName;
            stock.Symbol = stockModel.Symbol;
            await _context.SaveChangesAsync();

            return stock;

        }
    }
}
