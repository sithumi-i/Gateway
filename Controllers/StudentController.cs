using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gateway.BlindMatch.Data;
using Gateway.BlindMatch.Models;

namespace Gateway.BlindMatch.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var proposals = await _context.ProjectProposals
                .Where(p => p.StudentId == user.Id.ToString())
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            // Manually populate navigation properties
            var researchAreas = await _context.ResearchAreas.ToListAsync();
            foreach (var p in proposals)
            {
                p.ResearchArea = researchAreas.FirstOrDefault(r => r.Id == p.ResearchAreaId);
                if (p.SupervisorId != null)
                {
                    p.Supervisor = await _userManager.FindByIdAsync(p.SupervisorId);
                }
            }

            ViewBag.MatchedCount = proposals.Count(p => p.Status == ProjectStatus.Matched);
            ViewBag.PendingCount = proposals.Count(p => p.Status != ProjectStatus.Matched);
            ViewBag.UserName = user.FullName;

            return View(proposals);
        }
    }
}
