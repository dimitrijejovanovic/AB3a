using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB3.Models.DTO
{
    public class CreateProjectDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }
        public virtual List<String> Categories { get; set; }
        public Double Price { get; set; }
        public int UnitsInStock { get; set; }
        public List<IFormFile> Images { get; set; }
        public IFormFile CoverImage { get; set; }
    }
}
