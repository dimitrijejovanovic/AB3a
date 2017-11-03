using AB3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AB3.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }
        public DateTime? CreationDate { get; set; }
        public virtual ICollection<ProjectCategory> ProjectCategories { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public bool IsActive { get; set; }
        public int ViewCount { get; set; }
        public Double Price { get; set; }
        public int UnitsInStock { get; set; }

        public Project() {
            Name = "Untitled";
            IsActive = true;
            Year = DateTime.Now.Year.ToString();
            CreationDate = DateTime.Now;
        }

        
        //full project
        public Project(string name = "Untitled",
                       string desc = null,
                       string year = null,
                       ICollection<ProjectCategory> cats = null,
                       ICollection<Image> imgs = null,
                       Double prc = 0.0,
                       int uis = 0)
        {
            Name = name;
            Description = desc;
            ProjectCategories = cats;
            Images = imgs;
            Year = year;
            Price = prc;
            IsActive = true;
            UnitsInStock = uis;
            CreationDate = DateTime.Now;
        }
    }
}