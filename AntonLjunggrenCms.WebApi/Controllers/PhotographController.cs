using AntonLjunggrenCms.Core.Models;
using AntonLjunggrenCms.Core.Services;
using AntonLjunggrenCms.Data.Entites;
using AntonLjunggrenCms.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AntonLjunggrenCms.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PhotographController : ControllerBase
    {
        private readonly PhotographService _photographService;
        private readonly IFileService _fileService;

        public PhotographController(PhotographService photographService, IFileService fileService)
        {
            _photographService = photographService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var photograph = await _photographService.GetAll();

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var res = JsonConvert.SerializeObject(photograph.Where(p => p.IsPublished), serializerSettings);

            return Content(res, "application/json");
        }

        [HttpGet("photo/{id}/image/{*lores}")]
        public async Task<IActionResult> GetImage(string id, bool? lores)
        {
            try
            {
                var getLowRes = lores ?? false;

                var photo = await _photographService.Get(id);

                if(!photo.IsPublished)
                    return new OkObjectResult(false);

                var imageData = getLowRes ? photo.SdImageData : photo.HdImageData;
                var imageStream = await _fileService.GetFileContentStream(imageData.ImageFilePath);

                return new FileStreamResult(imageStream, imageData.FileContentType);
            }
            catch (Exception)
            {
                return new OkObjectResult(false);
            }
        }
    }
}
