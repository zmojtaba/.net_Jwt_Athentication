using System.ComponentModel.DataAnnotations.Schema;
using SecondApp.Dtos.Comment;
using SecondApp.Models;

namespace SecondApp.Dtos.Stock
{
    public class StockDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;


        public decimal purchase { get; set; }


        public decimal divdend { get; set; }

        public List<CommentDto>? Comments { get; set; }
    }
}
