using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AB3.Models;
using AB3.Models.DTO;
using System.IO;
using Microsoft.AspNetCore.Http;
using AB3.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AB3.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly AB3Context _context;

        public ProjectsController(AB3Context context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            return View(await _context.Project.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .SingleOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create(string uploadError = null)
        {
            var categories = _context.Category.ToList();
            ViewBag.Categories = categories;

            ViewBag.UploadError = uploadError;
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectDTO projectDTO)
        {
            Project project = new Project();
            if (ModelState.IsValid)
            {
                project.Name = projectDTO.Name;
                project.Description = projectDTO.Description;
                project.Price = projectDTO.Price;
                project.UnitsInStock = projectDTO.UnitsInStock;
                project.Year = projectDTO.Year;

                _context.Add(project);
                _context.SaveChanges();
                var lastProject = _context.Project.First(p => p.Name.Equals(projectDTO.Name));
                //categories part
                if (projectDTO.Categories != null && projectDTO.Categories.Count > 0)
                {
                    project.ProjectCategories = new List<ProjectCategory>();
                    foreach (var cat in projectDTO.Categories)
                    {
                        var category = _context.Category.First(c => c.CategoryName.Equals(cat));

                        var projectCatogory = new ProjectCategory();
                        projectCatogory.Project = lastProject;
                        projectCatogory.Category = category;
                        _context.Add(projectCatogory);
                        _context.SaveChanges();
                    }
                }
                //image part
                string[] allowedExtensions = { "png", "PNG", "jpg", "JPG", "jpeg", "JPEG" };
                long size = projectDTO.Images.Sum(f => f.Length);
                // full path to file in temp location
                var filePath = @"E:\AB3Uploads\";
                //for content images
                foreach (var formFile in projectDTO.Images)
                {
                    if (formFile.Length > 0)
                        if (!UploadImage(formFile, filePath, false, lastProject, allowedExtensions))
                        {
                            var error = "You can upload only image files.";
                            return RedirectToAction("Create", new { uploadError = error });
                        }
                }
                //for cover
                if (projectDTO.CoverImage.Length > 0)
                    if (!UploadImage(projectDTO.CoverImage, filePath, true, lastProject, allowedExtensions))
                    {
                        var error = "You can upload only image files.";
                        return RedirectToAction("Create", new { uploadError = error });
                    }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Create));
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.SingleOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,Year,CreationDate,IsActive,ViewCount,Price,UnitsInStock")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .SingleOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.SingleOrDefaultAsync(m => m.ProjectId == id);
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }

        private bool UploadImage(IFormFile file, string path, bool isCover, Project project, string[] allowedExtensions)
        {
            var guid = Guid.NewGuid().ToString();
            var extension = file.FileName.ToString().Split(".").Last();
            bool isValidExtension = false;
            foreach (var ext in allowedExtensions)
            {
                if (ext.Equals(extension))
                    isValidExtension = true;
            }
            if (isValidExtension)
                using (var stream = new FileStream(path + guid + "." + extension, FileMode.Create))
                {
                    file.CopyTo(stream);
                    Image image = new Image();
                    image.ImageName = guid;
                    image.Src = path;
                    image.Project = project;
                    image.IsCover = isCover;
                    _context.Add(image);
                    _context.SaveChanges();
                    return true;
                }
            else
                return false;
        }
    }
}
