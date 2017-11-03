using AB3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AB3.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<ProjectCategory> ProjectCategories { get; set; }

        public Category() { }
        public Category(string name)
        {
            CategoryName = name;
        }
    }
}