using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gateway.BlindMatch.Data;
using Gateway.BlindMatch.Models;

namespace Gateway.BlindMatch.Controllers
{
    [Authorize(Roles = "System Administrator,Module Leader")]
    public class ResearchAreasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResearchAreasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ResearchAreas.OrderBy(r => r.Name).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ResearchArea researchArea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(researchArea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(researchArea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var researchArea = await _context.ResearchAreas.FindAsync(id);
            if (researchArea == null)
                return NotFound();

            // Optionally check if any proposals reference this area
            var hasProposals = await _context.ProjectProposals
                .AnyAsync(p => p.ResearchAreaId == id);

            if (hasProposals)
            {
                TempData["Error"] = $"Cannot delete \"{researchArea.Name}\" — it has associated project proposals. Reassign them first.";
                return RedirectToAction(nameof(Index));
            }

            // Remove supervisor expertise entries that reference this area
            var expertiseEntries = _context.SupervisorExpertises.Where(se => se.ResearchAreaId == id);
            _context.SupervisorExpertises.RemoveRange(expertiseEntries);

            _context.ResearchAreas.Remove(researchArea);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
