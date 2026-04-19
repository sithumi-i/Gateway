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
            var proposals = await _context.ProjectProposals
                .Include(p => p.ResearchArea)
                .Include(p => p.Supervisor)
                .Where(p => p.StudentId == user!.Id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            ViewBag.MatchedCount  = proposals.Count(p => p.Status == ProjectStatus.Matched);
            ViewBag.PendingCount  = proposals.Count(p => p.Status == ProjectStatus.Pending);
            ViewBag.UserName      = user!.FullName;

            return View(proposals);
        }
    }
}
