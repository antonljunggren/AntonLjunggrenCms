using AntonLjunggrenCms.Core.Models;
using AntonLjunggrenCms.Data.Entites;
using AntonLjunggrenCms.Data.Persistance;
using AntonLjunggrenCms.Data.Services;
using AntonLjunggrenCms.Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonLjunggrenCms.Core.Services
{
    public sealed class PhotographService
    {
        private readonly IRepository<PhotographEntity, string> _photographRepository;
        private readonly IFileService _fileService;

        public PhotographService(IRepository<PhotographEntity, string> photographRepository, IFileService fileService)
        {
            _photographRepository = photographRepository;
            _fileService = fileService;
        }

        public async Task<Photograph> Get(string id)
        {
            var photo = await _photographRepository.GetAsync(id);

            if(photo == null)
            {
                throw new NullReferenceException($"Photograph with id: {id} does not exist!");
            }

            return Photograph.FromEntity(photo);
        }

        public async Task<List<Photograph>> GetAll()
        {
            var photos = (await _photographRepository.GetAllAsync()).ToList() ?? new List<PhotographEntity>();

            return photos.OrderBy(p => p.Order)
                .ThenBy(p => p.DateTaken)
                .Select(p => Photograph.FromEntity(p)).ToList();
        }
    
        public async Task<Photograph> AddPhotograph(Photograph photograph)
        {
            var res = await _photographRepository.AddAsync(Photograph.ToEntity(photograph));
            
            return Photograph.FromEntity(res);
        }

        public async Task DeletePhotograph(string photoId)
        {
            await _photographRepository.DeleteAsync(photoId);
        }

        public async Task<Photograph> UpdatePhotograph(Photograph newPhoto)
        {
            var photo = await _photographRepository.GetAsync(newPhoto.Id);

            if (photo == null)
            {
                throw new NullReferenceException($"Photograph with id: {newPhoto.Id} does not exist!");
            }

            photo.Title = newPhoto.Title;
            photo.Description = newPhoto.Description;
            photo.Location = newPhoto.Location;
            photo.FilmUsed = newPhoto.FilmUsed;
            photo.CameraUsed = newPhoto.CameraUsed;
            photo.Order = newPhoto.Order;
            photo.DateTaken = newPhoto.DateTaken;
            photo.HdImageData = newPhoto.HdImageData;
            photo.SdImageData = newPhoto.SdImageData;
            photo.IsPublished = newPhoto.IsPublished;

            var res = await _photographRepository.UpdateAsync(photo);

            return Photograph.FromEntity(res);
        }
    }
}
