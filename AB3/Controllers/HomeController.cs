using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AB3.Models;
using AB3.Data;
using Microsoft.EntityFrameworkCore;

namespace AB3.Controllers
{
    public class HomeController : Controller
    {

        private readonly AB3Context _context;

        public HomeController(AB3Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Category(string categoryName)
        {
            var projects = _context.Project
                       .Include(m => m.ProjectCategories)
                       .ThenInclude(pc => pc.Category)
                       .Include(m => m.Images)
                       .Where(m => m.ProjectCategories.Where(px => px.Category.CategoryName == categoryName).Count() > 0).ToList();

            return View(projects);
        }

        public IActionResult Project(int? id)
        {
            var project = _context.Project
                       .Include(m => m.Images)
                       .SingleOrDefault(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

    }
}
