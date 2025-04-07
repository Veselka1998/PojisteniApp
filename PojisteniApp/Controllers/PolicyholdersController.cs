using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PojisteniApp.Data;
using PojisteniApp.Models;

namespace PojisteniApp.Controllers
{
    public class PolicyholdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PolicyholdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Policyholders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Policyholders.ToListAsync());
        }

        // GET: Policyholders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            var policyholder = await _context.Policyholders
                .FirstOrDefaultAsync(m => m.Id == id);

            if (policyholder == null)
                return NotFound();

            var insurances = await _context.Insurances
                .Where(m => m.PolicyholderId == id)
                .ToListAsync();
            var viewModel = new DetailsViewModel
            {
                Policyholder = policyholder,
                Insurances = insurances
            };
            return View(viewModel);
        }

        // GET: Policyholders/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Policyholders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,Phone,StreetAddress,City,PostalCode")] Policyholder policyholder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(policyholder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(policyholder);
        }

        // GET: Policyholders/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policyholder = await _context.Policyholders.FindAsync(id);
            if (policyholder == null)
            {
                return NotFound();
            }
            return View(policyholder);
        }

        // POST: Policyholders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Phone,StreetAddress,City,PostalCode")] Policyholder policyholder)
        {
            if (id != policyholder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(policyholder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PolicyholderExists(policyholder.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(policyholder);
        }

        // GET: Policyholders/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policyholder = await _context.Policyholders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (policyholder == null)
            {
                return NotFound();
            }

            return View(policyholder);
        }

        // POST: Policyholders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var policyholder = await _context.Policyholders.FindAsync(id);
            if (policyholder != null)
            {
                _context.Policyholders.Remove(policyholder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PolicyholderExists(int id)
        {
            return _context.Policyholders.Any(e => e.Id == id);
        }
    }
}
