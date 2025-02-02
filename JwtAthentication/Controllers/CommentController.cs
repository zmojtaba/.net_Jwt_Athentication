using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecondApp.Dtos.Comment;
using SecondApp.Interfaces;
using SecondApp.Mappers;
using SecondApp.Models;

namespace SecondApp.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        public CommentController( ICommentRepository commentRepo )
        {
            _commentRepo = commentRepo;
        }



        [HttpGet]
        public async Task<IActionResult> GetAllCommentAsyncView()
        {
            IEnumerable<CommentDto> commentDto = await _commentRepo.GetAllCommentAsync();
            if (!commentDto.Any())
            {
                return NotFound("no comment yet");
            }
            return Ok(commentDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById([FromRoute] int id)
        {
            Comment? comment = await _commentRepo.GetCommentByIdAsync(id);

            if (comment == null)
            {
                return NotFound("No comment with this id was found");
            }

            return Ok(comment.ToCommentDto());

        }


        [HttpPost]
        [Route("{stockId}")]
        public async Task<IActionResult> CreateCommentView([FromRoute]int stockId , [FromBody] CreateCommentDto commentModel)
        {
            Console.WriteLine($"-------888------{stockId}");
            CommentDto commentDto = await _commentRepo.CreateAsync(stockId, commentModel);
            if (commentDto == null) return NotFound();
            return Ok(commentDto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateCommentView([FromRoute]int id, [FromBody] UpdateCommentDto commentModel)
        {
            Console.WriteLine($"------------{ id}");
            Comment? comment = await _commentRepo.UpdateAsync(id, commentModel);
            if (comment == null)
            {
                return NotFound("Model was not found!");
            }
            return Ok(comment.ToCommentDto());
        }

        [HttpGet("stock-comments/{stockId}")]
        public async Task<IActionResult> GetStockComment([FromRoute] int stockId)
        {
            IEnumerable<CommentDto> commentsDto = await _commentRepo.GetStockCommentByIdAsync(stockId);

            return Ok(commentsDto);
        }
    }
}
