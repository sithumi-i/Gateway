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
            
            // Get supervisor's expertise areas
            var expertiseIds = await _context.SupervisorExpertises
                .Where(se => se.SupervisorId == user.Id)
                .Select(se => se.ResearchAreaId)
                .ToListAsync();

            // Fetch proposals matching the expertise that are pending
            var proposals = await _context.ProjectProposals
                .Include(p => p.ResearchArea)
                .Include(p => p.Student) // We fetch student but will hide it in the View!
                .Where(p => expertiseIds.Contains(p.ResearchAreaId) && p.Status == ProjectStatus.Pending)
                .ToListAsync();

            var myMatchedProposals = await _context.ProjectProposals
                .Include(p => p.ResearchArea)
                .Include(p => p.Student)
                .Where(p => p.SupervisorId == user.Id)
                .ToListAsync();

            ViewBag.MatchedProposals = myMatchedProposals;
            return View(proposals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Match(int id)
        {
            var proposal = await _context.ProjectProposals.FindAsync(id);
            if (proposal == null || proposal.Status != ProjectStatus.Pending)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            proposal.SupervisorId = user.Id;
            proposal.Status = ProjectStatus.Matched;

            _context.Update(proposal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Expertise()
        {
            ViewData["ResearchAreaId"] = new SelectList(_context.ResearchAreas, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpertise(int researchAreaId)
        {
            var user = await _userManager.GetUserAsync(User);
            
            var exists = await _context.SupervisorExpertises
                .AnyAsync(se => se.SupervisorId == user.Id && se.ResearchAreaId == researchAreaId);

            if (!exists)
            {
                var se = new SupervisorExpertise
                {
                    SupervisorId = user.Id,
                    ResearchAreaId = researchAreaId
                };
                _context.SupervisorExpertises.Add(se);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
