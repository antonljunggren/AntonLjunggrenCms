namespace AntonLjunggrenCms.WebApp.Models
{
    public sealed class PhotographFormModel
    {
        public IFormFile? ImageFile { get; set; }

        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FilmUsed { get; set; } = string.Empty;
        public string CameraUsed { get; set; } = string.Empty;
        public DateTime DateTaken { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? Province { get; set; }
        public string? City { get; set; }
        public bool Published { get; set; }
    }
}
