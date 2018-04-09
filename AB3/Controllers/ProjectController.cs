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
        public IActionResult Index()
        {
            return View(_context.Project.ToList());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = _context.Project
                       .Include(m => m.ProjectCategories)
                       .ThenInclude(pc => pc.Category)
                       .Include(m => m.Images)
                       .Single(m => m.ProjectId == id);

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
        public async Task<IActionResult> Create(ProjectDTO projectDTO)
        {
            Project project = new Project();
            if (ModelState.IsValid)
            {
                try
                {
                    PopulateProject(projectDTO, project);
                    await _context.Project.AddAsync(project);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Create", new { uploadError = ex.Message });
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

            //jebem ti retardirani entiti u sve rupe
            var project = _context.Project
                .Include(m => m.ProjectCategories)
                .ThenInclude(pc => pc.Category)
                .Include(m => m.Images)
                .SingleOrDefault(m => m.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            ProjectDTO projectDTO = new ProjectDTO(project);

            ViewBag.Categories = _context.Category.ToList();
            return View(projectDTO);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ProjectDTO projectDTO)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Categories = _context.Category.ToList();
            if (ModelState.IsValid)
            {
                try
                {
                    var project = _context.Project
                        .Include(m => m.ProjectCategories)
                        .ThenInclude(pc => pc.Category)
                        .Include(m => m.Images)
                        .Single(m => m.ProjectId == id);

                    PopulateProject(projectDTO, project);

                    _context.Update(project);
                    await _context.SaveChangesAsync();
                    projectDTO = new ProjectDTO(project);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                    return View(projectDTO);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(projectDTO);
        }

        // GET: Projects/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = _context.Project
                .SingleOrDefault(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            project.IsActive = !project.IsActive;
            _context.Project.Update(project);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.SingleOrDefaultAsync(m => m.ProjectId == id);
            project.IsActive = false;
            _context.Project.Update(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async void PopulateProject(ProjectDTO projectDTO, Project project)
        {
            project.Name = projectDTO.Name;
            project.Description = projectDTO.Description;
            project.Price = projectDTO.Price;
            project.UnitsInStock = projectDTO.UnitsInStock;
            project.Year = projectDTO.Year;

            //categories part
            _context.ProjectCategory.RemoveRange(_context.ProjectCategory.Where(pc => pc.Project == project));
            _context.SaveChanges();

            if (projectDTO.Categories != null && projectDTO.Categories.Count > 0)
                project.ProjectCategories = new List<ProjectCategory>();
            {
                foreach (var cat in projectDTO.Categories)
                {
                    var category = _context.Category.First(c => c.CategoryName.Equals(cat));
                    var projectCatogory = new ProjectCategory
                    {
                        Project = project,
                        Category = category
                    };
                    // if (_context.ProjectCategory.FirstOrDefault(pc => pc == projectCatogory) == null)
                    project.ProjectCategories.Add(projectCatogory);
                }
            }

            //image part
            //allowing update of images
            if (project.Images != null)
            {
                if (projectDTO.FileCoverImage != null)
                {
                    project.Images.Remove(
                        project.Images.SingleOrDefault(i => i.IsCover));
                }
                if (projectDTO.FileContentImages != null)
                {

                    for (int i = 0; i < project.Images.Count; i++)
                    {
                        if (!project.Images.ToList()[i].IsCover)
                            project.Images.RemoveAt(i);
                    }

                }

            }
            //create new
            else
            {
                project.Images = new List<Image>();
            }

            string[] allowedExtensions = { "png", "PNG", "jpg", "JPG", "jpeg", "JPEG" };
            // full path to file in temp location
            var filePath = "wwwroot/images/uploads/";
            //for content images
            if (projectDTO.FileContentImages != null && projectDTO.FileContentImages.Count > 0)
                foreach (var formFile in projectDTO.FileContentImages)
                {
                    if (formFile.Length > 0)
                        if (!UploadImage(formFile, filePath, false, project, allowedExtensions))
                        {
                            throw new Exception("You can upload only image files.");
                        }
                }
            //for cover
            if (projectDTO.FileCoverImage != null && projectDTO.FileCoverImage.Length > 0)
                if (!UploadImage(projectDTO.FileCoverImage, filePath, true, project, allowedExtensions))
                {
                    throw new Exception("You can upload only image files.");
                }
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
                    image.ImageName = guid + "." + extension;
                    image.Src = path;
                    image.Project = project;
                    image.IsCover = isCover;

                    project.Images.Add(image);

                    //_context.Image.Add(image);
                    //_context.SaveChanges();
                    return true;
                }
            else
                return false;
        }
    }
}
