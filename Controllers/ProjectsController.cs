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
        public async Task<IActionResult> Create([Bind("Title,Abstract,TechnicalStack,ResearchAreaId")] ProjectProposal projectProposal)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();

                projectProposal.StudentId = user.Id.ToString();
                projectProposal.Status = ProjectStatus.Pending;

                _context.Add(projectProposal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ResearchAreas = _context.ResearchAreas.ToList();
            return View(projectProposal);
        }
    }
}
