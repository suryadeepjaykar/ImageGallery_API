using ImageGallery.API.Data;
using ImageGallery.API.Dtos;
using ImageGallery.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace ImageGallery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ImagesController(ApplicationDbContext context)
        {
                _context = context;
        }
        [HttpGet]
        public ActionResult<List<ImageItem>> GetAllImages([FromQuery]int page = 1, [FromQuery] int pagesize=10)
        {
            var skip = (page - 1) * pagesize;
            var images = _context.ImageItems.
                Skip(skip).Take(pagesize).ToList();
            return images;
        }

        [HttpPost]
        public async Task<IActionResult> PostImage([FromForm]ImageItemDto dto)
        {
            string ImagePath = await SaveImages(dto.ImageUrl);
            var imageItem = new ImageItem
            {
                Title = dto.Title,
                ImageUrl = ImagePath 
            };
            _context.ImageItems.Add(imageItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetImageById), new { id = imageItem.Id},imageItem);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImageById(int id)
        {
            var imageItem = await _context.ImageItems.FindAsync(id);
            if (imageItem == null)
            {
                return NotFound();
            }
            return Ok(imageItem);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage(int id, [FromForm]ImageItemDto dto)
        {
            var imageItem = await _context.ImageItems.FindAsync(id);
            if (imageItem == null)
            {
                return NotFound();
            }
            if (dto.ImageUrl != null) {
                string ImagePath = await SaveImages(dto.ImageUrl);
                imageItem.ImageUrl = ImagePath;
            }
            imageItem.Title = dto.Title;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id, [FromForm] ImageItemDto dto)
        {
            var imageItem = await _context.ImageItems.FindAsync(id);
            if (imageItem == null)
            {
                return NotFound();
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageItem.ImageUrl.TrimStart());
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.ImageItems.Remove(imageItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        private async Task<string> SaveImages(IFormFile? imageUrl)
        {
            var UploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(UploadFolder))
            { 
                Directory.CreateDirectory(UploadFolder);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(imageUrl.FileName);
            var filePath = Path.Combine(UploadFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageUrl.CopyToAsync(stream);
            }
            return $"/images/{fileName}";
        }
    }
}
