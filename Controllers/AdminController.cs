using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gateway.BlindMatch.Data;
using Gateway.BlindMatch.Models;

namespace Gateway.BlindMatch.Controllers
{
    [Authorize(Roles = "System Administrator,Module Leader")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var proposals = await _context.ProjectProposals
                .Include(p => p.Student)
                .Include(p => p.Supervisor)
                .Include(p => p.ResearchArea)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var totalStudents = await _userManager.GetUsersInRoleAsync("Student");
            var totalSupervisors = await _userManager.GetUsersInRoleAsync("Supervisor");
            var matchedCount = proposals.Count(p => p.Status == ProjectStatus.Matched);

            ViewBag.TotalStudents = totalStudents.Count;
            ViewBag.TotalSupervisors = totalSupervisors.Count;
            ViewBag.TotalProposals = proposals.Count;
            ViewBag.MatchedCount = matchedCount;
            ViewBag.MatchPercent = proposals.Count > 0
                ? Math.Round((double)matchedCount / proposals.Count * 100, 1)
                : 0;

            return View(proposals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reassign(int proposalId)
        {
            var proposal = await _context.ProjectProposals.FindAsync(proposalId);
            if (proposal != null)
            {
                proposal.SupervisorId = null;
                proposal.Status = ProjectStatus.Pending;
                _context.Update(proposal);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
