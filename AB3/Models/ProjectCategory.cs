using AB3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB3.Models
{
    public class ProjectCategory
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
