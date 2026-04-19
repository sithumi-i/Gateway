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
            return View(await _context.ResearchAreas.ToListAsync());
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
    }
}
