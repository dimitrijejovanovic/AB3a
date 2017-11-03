using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AB3.Models
{
    public class Image
    {
        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public bool IsCover { get; set; }
        public string Src { get; set; }
        public virtual Project Project { get; set; }

    }
}