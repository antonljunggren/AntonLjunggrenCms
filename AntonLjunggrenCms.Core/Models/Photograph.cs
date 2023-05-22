using AntonLjunggrenCms.Data.Entites;
using AntonLjunggrenCms.Data.ValueObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonLjunggrenCms.Core.Models
{
    public sealed class Photograph
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilmUsed { get; set; }
        public string CameraUsed { get; set; }
        [JsonIgnore]
        public int Order { get; set; }
        public DateOnly DateTaken { get; set; }
        [JsonIgnore]
        public bool IsPublished { get; set; }
        public Location Location { get; set; }
        public bool IsLandscape { get; set; }
        [JsonIgnore]
        public PhotoData HdImageData { get; set; }
        [JsonIgnore]
        public PhotoData SdImageData { get; set; }

        public Photograph(string id, string title, string description, string filmUsed, string cameraUsed, int order, DateOnly dateTaken, bool isPublished, Location location, bool isLandscape, PhotoData hdImageData, PhotoData sdImageData)
        {
            Id = id;
            Title = title;
            Description = description;
            FilmUsed = filmUsed;
            CameraUsed = cameraUsed;
            Order = order;
            DateTaken = dateTaken;
            IsPublished = isPublished;
            Location = location;
            IsLandscape = isLandscape;
            HdImageData = hdImageData;
            SdImageData = sdImageData;
        }

        public static Photograph FromEntity(PhotographEntity entity)
        {
            return new Photograph(
                entity.Id,
                entity.Title,
                entity.Description,
                entity.FilmUsed,
                entity.CameraUsed,
                entity.Order,
                entity.DateTaken,
                entity.IsPublished,
                entity.Location,
                entity.IsLandscape,
                entity.HdImageData,
                entity.SdImageData);
        }

        public static PhotographEntity ToEntity(Photograph photo)
        {
            return new PhotographEntity()
            {
                Id = photo.Id,
                Title = photo.Title,
                Description = photo.Description,
                FilmUsed = photo.FilmUsed,
                CameraUsed = photo.CameraUsed,
                Order = photo.Order,
                DateTaken = photo.DateTaken,
                IsPublished = photo.IsPublished,
                Location = photo.Location,
                IsLandscape = photo.IsLandscape,
                HdImageData = photo.HdImageData,
                SdImageData = photo.SdImageData
            };
        }
    }
}
