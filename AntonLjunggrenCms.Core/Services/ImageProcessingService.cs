using System;
using System.Collections.Generic;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using AntonLjunggrenCms.Core.Models;
using System.Drawing.Imaging;

namespace AntonLjunggrenCms.Core.Services
{
    public sealed class ImageProcessingService
    {
        private readonly int _hdSize = 1440, _loresSize = 720;

        public byte[] ConvertImageTo(Stream image, IImageEncoder encoder, out bool isLandscape)
        {
            using (var importedImage = Image.Load(image))
            {
                isLandscape = importedImage.Width > importedImage.Height;

                using (var convertedStream = new MemoryStream())
                {
                    importedImage.Save(convertedStream, encoder);
                    return convertedStream.ToArray();
                }
            }
        }

        public byte[] ResizeToLores(byte[] image)
        {
            return ResizeImageTo(image, _loresSize);
        }

        public byte[] ResizeToHd(byte[] image)
        {
            return ResizeImageTo(image, _hdSize);
        }

        private byte[] ResizeImageTo(byte[] image, int size) 
        {
            using (var stream = new MemoryStream(image))
            {
                return ResizeImageTo(stream, size);
            }
        }

        private byte[] ResizeImageTo(Stream image, int size)
        {
            using (var importedImage = Image.Load(image))
            {
                int newHeight = 0, newWidth = 0;

                if (Math.Max(importedImage.Width, importedImage.Height) > size)
                {
                    newWidth = importedImage.Width > importedImage.Height ?
                        size : (int)MathF.Floor(
                            ((float)importedImage.Width / (float)importedImage.Height) * size);

                    newHeight = importedImage.Width < importedImage.Height ?
                        size : (int)MathF.Floor(
                            ((float)importedImage.Height / (float)importedImage.Width) * size);
                }
                else
                {
                    newWidth = importedImage.Width;
                    newHeight = importedImage.Height;
                }

                importedImage.Mutate(i => i.Resize(newWidth, newHeight));
                using (var convertedStream = new MemoryStream())
                {
                    importedImage.Save(convertedStream, importedImage.Metadata.DecodedImageFormat ?? JpegFormat.Instance);
                    return convertedStream.ToArray();
                }
            }
        }
    
        public byte[] AddWatermark(byte[] image)
        {
            FontFamily ff = new FontCollection().Add("./wwwroot/arial.ttf");
            string watermarkText = "antonljunggren.se";
            var fontSize = 200f;

            using(var source = Image.Load(image))
            {
                Font watermarkFont = ff.CreateFont((int)Math.Floor(fontSize), FontStyle.BoldItalic);
                TextOptions watermarkTextOpt = new(watermarkFont)
                {
                    Origin = new PointF(0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };

                Color watermarkColor = new Color(new Vector4(1, 1, 1, 0.4f));

                float watermarkAngle = (float)(35 * Math.PI / 180);

                if(source.Width < _hdSize)
                {
                    watermarkAngle = (float)(60 * Math.PI / 180);
                }

                var watermarkTextSize = TextMeasurer.Measure(watermarkText, watermarkTextOpt);

                int watermarkWidth = (int)watermarkTextSize.Width;
                int watermarkHeight = (int)watermarkTextSize.Height;

                //rotated image that only contains watermark text
                using (Image<Rgba32> rotatedImage = new Image<Rgba32>(watermarkWidth, watermarkHeight))
                {
                    PointF center = new PointF(watermarkWidth / 2f, watermarkHeight / 2f);

                    rotatedImage.Mutate(i =>
                    {
                        i.Clear(Color.Transparent);
                        i.DrawText(watermarkTextOpt, watermarkText, watermarkColor);
                        i.Rotate((float)(watermarkAngle * (180 / Math.PI)));
                    });

                    //final iamge that the watermark will be added over
                    using (Image<Rgba32> finalImage = new Image<Rgba32>(source.Width, source.Height))
                    {
                        finalImage.Mutate(i =>
                        {
                            i.DrawImage(source, new Point(0, 0), 1f);
                        });

                        finalImage.Mutate(i =>
                        {
                            i.DrawImage(rotatedImage, new Point((finalImage.Width - rotatedImage.Width) / 2, (finalImage.Height - rotatedImage.Height) / 2), 1f);
                        });

                        using (var convertedStream = new MemoryStream())
                        {
                            finalImage.Save(convertedStream, source.Metadata.DecodedImageFormat ?? JpegFormat.Instance);
                            return convertedStream.ToArray();
                        }
                    }
                }
            }
        }

        public byte[] AddExifData(byte[] image, Photograph photo)
        {
            using (var importedImage = Image.Load(image))
            {
                var exifProfile = new ExifProfile();
                exifProfile.SetValue(ExifTag.DateTimeOriginal, photo.DateTaken.ToString("yyyy-MM-dd"));
                exifProfile.SetValue(ExifTag.Copyright, $"© Anton Ljunggren - {DateTime.Now.Year}");
                exifProfile.SetValue(ExifTag.Artist, "Anton Ljunggren");
                exifProfile.SetValue(ExifTag.Model, photo.CameraUsed);
                importedImage.Metadata.ExifProfile = exifProfile;

                using (var convertedStream = new MemoryStream())
                {
                    importedImage.Save(convertedStream, importedImage.Metadata.DecodedImageFormat ?? JpegFormat.Instance);
                    return convertedStream.ToArray();
                }
            }
        }
    }
}
