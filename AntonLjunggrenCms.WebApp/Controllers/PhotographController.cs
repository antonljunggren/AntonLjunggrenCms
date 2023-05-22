using AntonLjunggrenCms.Core.Models;
using AntonLjunggrenCms.Core.Services;
using AntonLjunggrenCms.Data.Services;
using AntonLjunggrenCms.Data.Utils;
using AntonLjunggrenCms.Data.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats;
using AntonLjunggrenCms.WebApp.Models;
using AntonLjunggrenCms.Data.Persistance;

namespace AntonLjunggrenCms.WebApp.Controllers
{
    public class PhotographController : Controller
    {
        private readonly ILogger<PhotographController> _logger;
        private readonly PhotographService _photographService;
        private readonly ImageProcessingService _imageprocessingService;
        private readonly IFileService _fileService;

        public PhotographController(
            ILogger<PhotographController> logger, 
            PhotographService photographService, 
            ImageProcessingService imageprocessingService, 
            IFileService fileService)
        {
            _logger = logger;
            _photographService = photographService;
            _imageprocessingService = imageprocessingService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new PhotographViewModel();

            var photographs = await _photographService.GetAll();

            viewModel.photographs = photographs;

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(string? photoId)
        {
            if (string.IsNullOrWhiteSpace(photoId))
                return BadRequest();

            var photo = await _photographService.Get(photoId);

            return View(photo);
        }

        public async Task<IActionResult> GetPreviewImage(string id)
        {
            return await GetImage(id);
        }

        public async Task<IActionResult> GetHdImage(string id)
        {
            return await GetImage(id, false);
        }

        private async Task<IActionResult> GetImage(string id, bool lores = true)
        {
            var photo = await _photographService.Get(id);
            var photoData = lores ? photo.SdImageData : photo.HdImageData;

            var imageStream = await _fileService.GetFileContentStream(photoData.ImageFilePath);

            return new FileStreamResult(imageStream, photoData.FileContentType);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string id, bool raise, bool redirectEdit = false)
        {
            var updatePhoto = await _photographService.Get(id);
            updatePhoto.Order += raise ? 1 : -1;

            if (updatePhoto.Order < 0)
            {
                updatePhoto.Order = 0;
            }

            await _photographService.UpdatePhotograph(updatePhoto);

            if(redirectEdit)
            {
                return RedirectToAction(nameof(Edit), new { photoId = id });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [RequestSizeLimit(600000000)]
        public async Task<IActionResult> UpdatePhotograph(PhotographFormModel formModel)
        {
            var updatePhoto = await _photographService.Get(formModel.Id);
            updatePhoto.Title = formModel.Title;
            updatePhoto.Description = formModel.Description;
            updatePhoto.FilmUsed = formModel.FilmUsed;
            updatePhoto.CameraUsed = formModel.CameraUsed;
            updatePhoto.Location = new Location()
            {
                Country = formModel.Country,
                City = formModel.City,
                Province = formModel.Province
            };
            updatePhoto.IsPublished = formModel.Published;
            updatePhoto.DateTaken = DateOnly.FromDateTime(formModel.DateTaken);

            if (formModel.ImageFile != null && formModel.ImageFile.Length > 0)
            {
                IImageEncoder encoder = new JpegEncoder()
                {
                    Quality = 100
                };

                string loresFilename, hdFilename;
                bool imageIsLandscape = false;

                using (Stream stream = formModel.ImageFile.OpenReadStream())
                {
                    byte[] convertedImage = _imageprocessingService.ConvertImageTo(stream, encoder, out imageIsLandscape);
                    byte[] loresImage = _imageprocessingService.ResizeToLores(convertedImage);
                    byte[] hdImage = _imageprocessingService.ResizeToHd(convertedImage);
                    hdImage = _imageprocessingService.AddWatermark(hdImage);

                    using (var lrs = new MemoryStream(loresImage))
                    {
                        loresFilename = await _fileService.SaveFile(lrs, "image/jpeg");
                    }

                    using (var hds = new MemoryStream(hdImage))
                    {
                        hdFilename = await _fileService.SaveFile(hds, "image/jpeg");
                    }
                }

                updatePhoto.IsLandscape = imageIsLandscape;

                updatePhoto.HdImageData = new PhotoData
                {
                    FileContentType = "image/jpeg",
                    ImageFilePath = hdFilename
                };

                updatePhoto.SdImageData = new PhotoData
                {
                    FileContentType = "image/jpeg",
                    ImageFilePath = loresFilename
                };
            }

            await _photographService.UpdatePhotograph(updatePhoto);

            return RedirectToAction(nameof(Edit), new { photoId = formModel.Id });
        }

        [HttpPost]
        [RequestSizeLimit(600000000)]
        public async Task<IActionResult> AddPhotograph(PhotographFormModel formModel)
        {
            if (formModel.ImageFile == null || formModel.ImageFile.Length < 10)
            {
                return BadRequest("Image missing");
            }

            IImageEncoder encoder = new JpegEncoder()
            {
                Quality = 100
            };

            string loresFilename, hdFilename;
            bool imageIsLandscape = false;

            using (Stream stream = formModel.ImageFile.OpenReadStream())
            {
                byte[] convertedImage = _imageprocessingService.ConvertImageTo(stream, encoder, out imageIsLandscape);
                byte[] loresImage = _imageprocessingService.ResizeToLores(convertedImage);
                byte[] hdImage = _imageprocessingService.ResizeToHd(convertedImage);
                hdImage = _imageprocessingService.AddWatermark(hdImage);

                using (var lrs = new MemoryStream(loresImage))
                {
                    loresFilename = await _fileService.SaveFile(lrs, "image/jpeg");
                }

                using (var hds = new MemoryStream(hdImage))
                {
                    hdFilename = await _fileService.SaveFile(hds, "image/jpeg");
                }
            }

            var hdImageData = new PhotoData
            {
                FileContentType = "image/jpeg",
                ImageFilePath = hdFilename
            };

            var sdImageData = new PhotoData
            {
                FileContentType = "image/jpeg",
                ImageFilePath = loresFilename
            };

            var photograph = new Photograph(
                ShortStringId.Generate(11),
                formModel.Title,
                formModel.Description,
                formModel.FilmUsed,
                formModel.CameraUsed,
                0,
                DateOnly.FromDateTime(formModel.DateTaken),
                false,
                new Location()
                {
                    Country = formModel.Country,
                    City = formModel.City,
                    Province = formModel.Province
                },
                imageIsLandscape,
                hdImageData,
                sdImageData);

            var newPhoto = await _photographService.AddPhotograph(photograph);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhotograph(string? photoId)
        {
            if (string.IsNullOrWhiteSpace(photoId))
            {
                return BadRequest($"Wrong photo id: {photoId?.ToString()}");
            }

            await _photographService.DeletePhotograph(photoId);

            return RedirectToAction(nameof(Index));
        }
    }
}
