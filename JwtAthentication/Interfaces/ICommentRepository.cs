using System.Runtime.InteropServices;
using SecondApp.Dtos.Comment;
using SecondApp.Models;

namespace SecondApp.Interfaces
{
    public interface ICommentRepository
    {
        Task< IEnumerable<CommentDto> > GetAllCommentAsync();
        Task<Comment?>                  GetCommentByIdAsync(int id);

        Task<CommentDto>                CreateAsync(int sotckId ,CreateCommentDto commnetModel);

        Task<Comment>                   UpdateAsync(int CommentId, UpdateCommentDto commdentModel);

        Task<IEnumerable<CommentDto>>   GetStockCommentByIdAsync(int stockId);
        Task<bool>                      ExistingStock(int stockId);

    }
}
