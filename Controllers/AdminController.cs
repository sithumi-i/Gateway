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
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            // Manually populate navigation properties
            var researchAreas = await _context.ResearchAreas.ToListAsync();
            var studentIds = proposals.Select(p => p.StudentId).Distinct().ToList();
            var supervisorIds = proposals.Where(p => p.SupervisorId != null).Select(p => p.SupervisorId!).Distinct().ToList();
            var allUserIds = studentIds.Concat(supervisorIds).Distinct().ToList();

            var users = new Dictionary<string, ApplicationUser>();
            foreach (var uid in allUserIds)
            {
                var u = await _userManager.FindByIdAsync(uid);
                if (u != null) users[uid] = u;
            }

            foreach (var p in proposals)
            {
                p.ResearchArea = researchAreas.FirstOrDefault(r => r.Id == p.ResearchAreaId);
                if (users.TryGetValue(p.StudentId, out var student)) p.Student = student;
                if (p.SupervisorId != null && users.TryGetValue(p.SupervisorId, out var supervisor)) p.Supervisor = supervisor;
            }

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
            var proposal = await _context.ProjectProposals.FirstOrDefaultAsync(p => p.Id == proposalId);
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
