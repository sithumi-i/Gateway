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

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var myProjects = await _context.ProjectProposals
                .Include(p => p.ResearchArea)
                .Include(p => p.Supervisor) // Include to show if assigned
                .Where(p => p.StudentId == user.Id)
                .ToListAsync();

            return View(myProjects);
        }

        public IActionResult Create()
        {
            ViewBag.ResearchAreas = _context.ResearchAreas.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Abstract,TechnicalStack,ResearchAreaId")] ProjectProposal projectProposal)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                projectProposal.StudentId = user.Id;
                projectProposal.Status = ProjectStatus.Pending;

                _context.Add(projectProposal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResearchAreaId"] = new SelectList(_context.ResearchAreas, "Id", "Name", projectProposal.ResearchAreaId);
            return View(projectProposal);
        }
    }
}
