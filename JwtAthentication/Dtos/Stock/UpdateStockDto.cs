namespace SecondApp.Dtos.Stock
{
    public class UpdateStockDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public decimal purchase { get; set; }
        public decimal divdend { get; set; }
    }
}
