using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gateway.BlindMatch.Data;
using Gateway.BlindMatch.Models;

namespace Gateway.BlindMatch.Controllers
{
    [Authorize(Roles = "Student")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var myProjects = await _context.ProjectProposals
                .Where(p => p.StudentId == user.Id.ToString())
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            // Manually populate navigation properties
            var researchAreas = await _context.ResearchAreas.ToListAsync();
            foreach (var p in myProjects)
            {
                p.ResearchArea = researchAreas.FirstOrDefault(r => r.Id == p.ResearchAreaId);
                if (p.SupervisorId != null)
                {
                    p.Supervisor = await _userManager.FindByIdAsync(p.SupervisorId);
                }
            }

            return View(myProjects);
        }

        public IActionResult Create()
        {
            ViewBag.ResearchAreas = _context.ResearchAreas.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Abstract,TechnicalStack,ResearchAreaId")] ProjectProposal projectProposal,
            IFormFile? DocumentFile)
        {
            // Remove server-set fields from validation
            ModelState.Remove("StudentId");
            ModelState.Remove("Status");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();

                // Assign a unique ID since MongoDB doesn't auto-increment int keys
                var maxId = await _context.ProjectProposals.AnyAsync()
                    ? await _context.ProjectProposals.MaxAsync(p => p.Id)
                    : 0;
                projectProposal.Id = maxId + 1;

                projectProposal.StudentId = user.Id.ToString();
                projectProposal.Status = ProjectStatus.Pending;

                // Handle file upload
                if (DocumentFile != null && DocumentFile.Length > 0)
                {
                    // Validate file size (10MB max)
                    if (DocumentFile.Length > 10 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "File size must be less than 10MB.");
                        ViewBag.ResearchAreas = _context.ResearchAreas.ToList();
                        return View(projectProposal);
                    }

                    // Validate file type
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                    var extension = Path.GetExtension(DocumentFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Only PDF and DOCX files are allowed.");
                        ViewBag.ResearchAreas = _context.ResearchAreas.ToList();
                        return View(projectProposal);
                    }

                    // Save the file
                    var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsDir);

                    var uniqueName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsDir, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await DocumentFile.CopyToAsync(stream);
                    }

                    projectProposal.DocumentPath = $"/uploads/{uniqueName}";
                }

                _context.Add(projectProposal);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your proposal has been submitted successfully! It is now awaiting blind-match review.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ResearchAreas = _context.ResearchAreas.ToList();
            return View(projectProposal);
        }
    }
}
