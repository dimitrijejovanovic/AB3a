using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB3.Models.DTO
{
    public class ProjectDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }
        public virtual List<String> Categories { get; set; }
        public Double Price { get; set; }
        public int UnitsInStock { get; set; }
        public List<IFormFile> FileContentImages { get; set; }
        public IFormFile FileCoverImage { get; set; }
        public List<Image> ContentImages { get; set; }
        public Image CoverImage { get; set; }

        public ProjectDTO() { }

        public ProjectDTO(Project proj) {
            Name = proj.Name;
            Description = proj.Description;
            Price = proj.Price;
            UnitsInStock = proj.UnitsInStock;
            Year = proj.Year;

            Categories = proj.ProjectCategories.Select(c => (c.Category.CategoryName)).ToList();

            CoverImage = proj.Images.FirstOrDefault(ci => ci.IsCover == true);
            ContentImages = proj.Images.Where(i => i.IsCover == false).ToList();

        }
    }
}
