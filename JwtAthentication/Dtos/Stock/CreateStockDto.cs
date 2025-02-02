using SecondApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecondApp.Dtos.Stock
{
    public class CreateStockDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public decimal purchase { get; set; }
        public decimal divdend { get; set; }

    }
}
