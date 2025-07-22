using FileStorage.FileService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FileStorage.FileService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(IFormFileCollection files)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.UploadFilesAsync(files, userId);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.DownloadFileAsync(id, userId);

            if (result.Success)
                return File(result.Data, "application/octet-stream");

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.DeleteFileAsync(id, userId);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFiles([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.GetUserFilesAsync(userId, page, size);

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