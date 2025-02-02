namespace SecondApp.Dtos.Comment
{
    public class CreateCommentDto
    {
        public required string Content { get; set; }
        public string? Title{ get; set; }
    }
}
