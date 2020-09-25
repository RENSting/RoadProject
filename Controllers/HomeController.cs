using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Text;

using RoadProject.Models;

namespace RoadProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoadDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, RoadDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index(string flag)
        {
            ViewBag.Flag = string.IsNullOrWhiteSpace(flag) ? "1" : null;

            var query = from p in _dbContext.Project
                        where string.IsNullOrWhiteSpace(flag) || p.IsActive == true
                        orderby p.CreatedOn descending
                        select p;

            return View(await query.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var project = await _dbContext.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            var model = TemplateViewModel.CreateInstance(project);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id, string dir, string file)
        {
            string mime;
            if (file.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                mime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else if (file.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                mime = "application/vnd.ms-excel";
            }
            else if (file.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                mime = "application/vnd.ms-word";
            }
            else
            {
                return Error();
            }

            var project = await _dbContext.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            using var buffer = new System.IO.MemoryStream();
            Helper.CreateTemplateInBuffer(buffer, project, dir, file);
            return File(buffer.ToArray(), mime, file);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            Project project;
            if (id == null)
            {
                project = new Project { CreatedOn = DateTime.Today, IsActive = true };
            }
            else
            {
                project = await _dbContext.Project.FindAsync(id);
                if (project == null)
                {
                    return NotFound();
                }
            }

            return View(project);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Project model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    //update
                    _dbContext.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    _dbContext.Project.Add(model);
                }
                try
                {
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ModelState.AddModelError("", ex.ToString());
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.ToString());
                }
            }

            return View(model);
        }

        public IActionResult Extract()
        {
            string source = @"./Templates/original.zip";
            //string target = @"./Templates";

            using var zip = ZipFile.Open(source, ZipArchiveMode.Read, Encoding.GetEncoding("GBK"));

            StringBuilder outputBuilder = new StringBuilder();
            foreach (var entry in zip.Entries)
            {
                outputBuilder.Append(entry.ToString());
                outputBuilder.Append("\\r\\n");
            }


            return Content($"first file's entry name = {outputBuilder.ToString()}");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
