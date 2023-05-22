using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntonLjunggrenCms.Data.ValueObjects;

namespace AntonLjunggrenCms.Data.Entites
{
    public sealed class PhotographEntity
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FilmUsed { get; set; } = string.Empty;
        public string CameraUsed { get; set; } = string.Empty;
        public int Order { get; set; } = 0;
        public DateOnly DateTaken { get; set; }
        public bool IsPublished { get; set; }
        public Location Location { get; set; } = new();
        public bool IsLandscape { get; set; }
        public PhotoData HdImageData { get; set; } = new();
        public PhotoData SdImageData { get; set; } = new();
    }
}
