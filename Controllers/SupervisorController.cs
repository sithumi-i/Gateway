using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gateway.BlindMatch.Data;
using Gateway.BlindMatch.Models;

namespace Gateway.BlindMatch.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupervisorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Get supervisor's expertise areas
            var expertiseIds = await _context.SupervisorExpertises
                .Where(se => se.SupervisorId == user.Id.ToString())
                .Select(se => se.ResearchAreaId)
                .ToListAsync();

            // Fetch proposals matching expertise that are pending
            var proposals = await _context.ProjectProposals
                .Where(p => expertiseIds.Contains(p.ResearchAreaId) && p.Status == ProjectStatus.Pending)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            // Manually populate navigation properties
            var researchAreas = await _context.ResearchAreas.ToListAsync();
            foreach (var p in proposals)
            {
                p.ResearchArea = researchAreas.FirstOrDefault(r => r.Id == p.ResearchAreaId);
                // Student is loaded but hidden in view (blind matching)
                p.Student = await _userManager.FindByIdAsync(p.StudentId);
            }

            // Proposals this supervisor has already matched
            var myMatchedProposals = await _context.ProjectProposals
                .Where(p => p.SupervisorId == user.Id.ToString())
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            foreach (var p in myMatchedProposals)
            {
                p.ResearchArea = researchAreas.FirstOrDefault(r => r.Id == p.ResearchAreaId);
                p.Student = await _userManager.FindByIdAsync(p.StudentId);
            }

            ViewBag.MatchedProposals = myMatchedProposals;
            return View(proposals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Match(int id)
        {
            var proposal = await _context.ProjectProposals.FirstOrDefaultAsync(p => p.Id == id);
            if (proposal == null || proposal.Status != ProjectStatus.Pending)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            proposal.SupervisorId = user.Id.ToString();
            proposal.Status = ProjectStatus.Matched;

            _context.Update(proposal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Show the Expertise management page
        public async Task<IActionResult> Expertise()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            ViewData["ResearchAreaId"] = new SelectList(_context.ResearchAreas, "Id", "Name");

            // Get current expertise areas for this supervisor
            var myExpertiseIds = await _context.SupervisorExpertises
                .Where(se => se.SupervisorId == user.Id.ToString())
                .Select(se => se.ResearchAreaId)
                .ToListAsync();

            var myExpertise = await _context.ResearchAreas
                .Where(r => myExpertiseIds.Contains(r.Id))
                .ToListAsync();

            ViewBag.MyExpertise = myExpertise;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpertise(int researchAreaId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var exists = await _context.SupervisorExpertises
                .AnyAsync(se => se.SupervisorId == user.Id.ToString() && se.ResearchAreaId == researchAreaId);

            if (!exists)
            {
                var se = new SupervisorExpertise
                {
                    SupervisorId = user.Id.ToString(),
                    ResearchAreaId = researchAreaId
                };
                _context.SupervisorExpertises.Add(se);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Expertise));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveExpertise(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var entry = await _context.SupervisorExpertises
                .FirstOrDefaultAsync(se => se.SupervisorId == user.Id.ToString() && se.ResearchAreaId == id);

            if (entry != null)
            {
                _context.SupervisorExpertises.Remove(entry);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Expertise));
        }
    }
}
