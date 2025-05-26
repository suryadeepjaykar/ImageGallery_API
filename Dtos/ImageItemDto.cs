using Microsoft.AspNetCore.Http;

namespace ImageGallery.API.Dtos
{
    public class ImageItemDto
    {
        public string? Title { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}

