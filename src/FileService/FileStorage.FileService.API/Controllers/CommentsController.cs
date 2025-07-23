using FileStorage.FileService.Application.DTOs;
using FileStorage.FileService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FileStorage.FileService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(AddCommentRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _commentService.AddCommentAsync(request, userId);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("file/{fileId}")]
        public async Task<IActionResult> GetFileComments(int fileId)
        {
            var result = await _commentService.GetFileCommentsAsync(fileId);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _commentService.DeleteCommentAsync(id, userId);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0; // Or throw an exception
        }
    }
}