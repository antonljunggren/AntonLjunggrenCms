using AntonLjunggrenCms.Data.Entites;
using AntonLjunggrenCms.Data.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonLjunggrenCms.Data.Persistance
{
    public sealed class PhotographRepository: IRepository<PhotographEntity, string>
    {
        private readonly EfContext _context;

        public PhotographRepository(IDbContextFactory<EfContext> contextFactory)
        {
            _context = contextFactory.CreateDbContext();
        }

        public async Task<PhotographEntity> AddAsync(PhotographEntity photo)
        {
            photo.Id = ShortStringId.Generate(11);

            var res = await _context.Photographs.AddAsync(photo);
            await _context.SaveChangesAsync();

            return res.Entity;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var photo = await _context.Photographs.SingleOrDefaultAsync(p => p.Id == id);

            if (photo == null)
            {
                throw new NullReferenceException($"Photograph with id: {id} does not exist!");
            }

            _context.Photographs.Remove(photo);
            var res = await _context.SaveChangesAsync();

            return res > 0;
        }

        public async Task<IEnumerable<PhotographEntity>> GetAllAsync()
        {
            return await _context.Photographs.AsNoTracking().ToListAsync();
        }

        public async Task<PhotographEntity> GetAsync(string id)
        {
            var photograph = await _context.Photographs.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
            if (photograph == null)
            {
                throw new NullReferenceException($"Photograph with id: {id} does not exist!");
            }

            return photograph;
        }

        public async Task<PhotographEntity> UpdateAsync(PhotographEntity newPhoto)
        {
            var photo = await _context.Photographs.SingleOrDefaultAsync(p => p.Id == newPhoto.Id);

            if (photo == null)
            {
                throw new NullReferenceException($"Photograph with id: {newPhoto.Id} does not exist!");
            }

            photo.Title = newPhoto.Title;
            photo.Description = newPhoto.Description;
            photo.Location = newPhoto.Location;
            photo.FilmUsed = newPhoto.FilmUsed;
            photo.CameraUsed = newPhoto.CameraUsed;
            photo.DateTaken = newPhoto.DateTaken;
            photo.Order = newPhoto.Order;
            photo.HdImageData = newPhoto.HdImageData;
            photo.SdImageData = newPhoto.SdImageData;
            photo.IsPublished = newPhoto.IsPublished;

            var res = _context.Photographs.Update(photo);
            await _context.SaveChangesAsync();

            return res.Entity;
        }
    }
}
