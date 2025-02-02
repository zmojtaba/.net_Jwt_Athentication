using SecondApp.Dtos.Stock;
using SecondApp.Models;

namespace SecondApp.Mappers
{
    public static class StockMappers
    {
        public static StockDto ToStockDto ( this Stock stockModel)
        {
            return new StockDto
            {
                Id = stockModel.Id,
                CompanyName = stockModel.CompanyName,
                divdend = stockModel.divdend,
                purchase = stockModel.purchase,
                Symbol = stockModel.Symbol,
                Comments = stockModel.Comments.Select( c => c.ToCommentDto() ).ToList()
            };
        }
    }
}
