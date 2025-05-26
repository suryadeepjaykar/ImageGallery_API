namespace ImageGallery.API.Models
{
    public class ImageItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public required string ImageUrl { get;  set; }
    }
}
