using FileStorage.FileService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FileStorage.FileService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileSharesController : ControllerBase
    {
        private readonly IFileShareService _fileShareService;

        public FileSharesController(IFileShareService fileShareService)
        {
            _fileShareService = fileShareService;
        }

        [HttpPost("create-link")]
        public async Task<IActionResult> CreateShareLink(int fileId, string email)
        {
            var userId = GetCurrentUserId();
            var result = await _fileShareService.CreateShareLinkAsync(fileId, email, userId);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptShare([FromBody] string shareToken)
        {
            var result = await _fileShareService.AcceptShareAsync(shareToken);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("reject")]
        public async Task<IActionResult> RejectShare([FromBody] string shareToken)
        {
            var result = await _fileShareService.RejectShareAsync(shareToken);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSharedFiles()
        {
            var userId = GetCurrentUserId();
            var result = await _fileShareService.GetSharedFilesAsync(userId);

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