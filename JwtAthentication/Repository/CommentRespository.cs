using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecondApp.Data;
using SecondApp.Dtos.Comment;
using SecondApp.Interfaces;
using SecondApp.Mappers;
using SecondApp.Models;

namespace SecondApp.Repository
{
    public class CommentRespository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRespository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CommentDto> CreateAsync(int stockId, CreateCommentDto commnetModel)
        {
            Console.WriteLine($"-------------{stockId}");
            if (!await ExistingStock(stockId)) return null;

            // convert createCommentDto to Comment model with mappers:

            Comment comment = commnetModel.ToCommentFromCreate(stockId);
            Stock stock = await _context.Stocks.Include(s => s.Comments).FirstOrDefaultAsync(s => s.Id == stockId);
            stock.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return comment.ToCommentDto();
        }

        public async Task<bool> ExistingStock(int stockId)
        {
            return await _context.Stocks.AnyAsync(s => s.Id == stockId);
        }

        public async Task<IEnumerable<CommentDto>> GetAllCommentAsync()
        {
            List<Comment> comments = await _context.Comments.ToListAsync();
            IEnumerable<CommentDto> CommentDtos = comments.Select(c => c.ToCommentDto());
            return CommentDtos;
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            Comment? comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (comment == null)
            {
                return null;
            }
            return comment;
        }

        public async Task<IEnumerable<CommentDto>> GetStockCommentByIdAsync(int stockId)
        {
            bool isStock = await ExistingStock(stockId);
            //Stock? stock = await _context.Stocks.Include(s => s.Comments).FirstOrDefaultAsync(s => s.Id == stockId);
            if (!isStock) return null;
            List<Comment> comments = await _context.Comments.Where(c => c.StockID == stockId).ToListAsync();
            IEnumerable<CommentDto> commentsDto = comments.Select(c => c.ToCommentDto());
            return commentsDto;
        }

        public async Task<Comment> UpdateAsync(int id ,UpdateCommentDto commentModel)
        {
            Comment? comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (comment == null)
            {
                return null;
            }

            comment.Content = commentModel.Content;

            await _context.SaveChangesAsync();

            return comment;
            
        }

    }
}
