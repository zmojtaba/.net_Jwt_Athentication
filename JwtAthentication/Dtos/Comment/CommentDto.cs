namespace SecondApp.Dtos.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int? StockID { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
