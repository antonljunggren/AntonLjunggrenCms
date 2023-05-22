using AntonLjunggrenCms.Data.Entites;
using AntonLjunggrenCms.Data.Persistance;
using AntonLjunggrenCms.Data.Services;
using System.Collections;

namespace AntonLjunggrenCms.WebEditor.Data
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

        public async Task<List<PhotographEntity>> GetPhotos()
        {
            return (await _photographRepository.GetAllAsync()).ToList();
        }

        public async Task<string> GetImage(string photoId, bool hdImage = false)
        {
            var photo = await _photographRepository.GetAsync(photoId);
            if (photo == null) return "";
            var photoData = hdImage ? photo.HdImageData : photo.SdImageData;

            using var imageStream = await _fileService.GetFileContentStream(photoData.ImageFilePath);
            using MemoryStream ms = new MemoryStream();

            int read = 0;
            byte[] buffer = new byte[1024];
            while((read = imageStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            var bytes = ms.ToArray();
            var b64String = Convert.ToBase64String(bytes);
            return "data:image/png;base64," + b64String;
        }
    }
}
