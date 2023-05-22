using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonLjunggrenCms.Data.ValueObjects
{
    public sealed class Location
    {
        public string Country { get; set; } = string.Empty;
        public string? Province { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
    }
}
