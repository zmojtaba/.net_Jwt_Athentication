using SecondApp.Dtos.Comment;
using SecondApp.Models;

namespace SecondApp.Mappers
{
    public static class CommentMappers
    {
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
            return new CommentDto
            {
                Id = commentModel.Id,
                Content = commentModel.Content,
                Title = commentModel.Title,
                CreatedOn = commentModel.CreatedOn,
                StockID = commentModel.StockID
            };

        }

        public static Comment ToCommentFromCreate(this CreateCommentDto commentModel, int stockId)
        {
            return new Comment
            {
                Title       = commentModel.Title,
                Content     = commentModel.Content,
                StockID     = stockId
            };
        }
    }
}
