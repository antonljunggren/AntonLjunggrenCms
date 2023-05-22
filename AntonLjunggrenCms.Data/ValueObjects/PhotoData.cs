using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonLjunggrenCms.Data.ValueObjects
{
    public sealed class PhotoData
    {
        public string ImageFilePath { get; set; } = string.Empty;
        public string FileContentType { get; set; } = string.Empty;
    }
}
