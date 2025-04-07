using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PojisteniApp.Data;
using PojisteniApp.Models;

namespace PojisteniApp.Controllers
{
    public class InsurancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsurancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Insurances
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Insurances.Include(i => i.Policyholder);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Insurances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _context.Insurances
                .Include(i => i.Policyholder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        // GET: Insurances/Create
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return NotFound();

            var policyholder = await _context.Policyholders
                .FirstOrDefaultAsync(m => m.Id == id);

            if (policyholder == null)
                return NotFound();

            var newInsurance = new Insurance
            {
                PolicyholderId = policyholder.Id
            };

            ViewBag.PolicyholderName = $"{policyholder.LastName} {policyholder.FirstName}";
            return View(newInsurance);
        }

        // POST: Insurances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Name,InsuranceObject,Price,EffectiveDate,ExpirationDate,PolicyholderId")] Insurance insurance)
        {
            if (ModelState.IsValid)
            {
                _context.Insurances.Add(insurance);
                await _context.SaveChangesAsync();
                return RedirectTo("Details", "Policyholders", insurance.PolicyholderId);
            }
            ViewData["PolicyholderId"] = new SelectList(_context.Policyholders, "Id", "Id", insurance.PolicyholderId);
            return View(insurance);
        }

        // GET: Insurances/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _context.Insurances.FindAsync(id);
            if (insurance == null)
            {
                return NotFound();
            }
            ViewData["PolicyholderId"] = new SelectList(_context.Policyholders, "Id", "Id", insurance.PolicyholderId);
            return View(insurance);
        }

        // POST: Insurances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,InsuranceObject,Price,EffectiveDate,ExpirationDate,PolicyholderId")] Insurance insurance)
        {
            if (id != insurance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insurance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceExists(insurance.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectTo("Details", "Policyholders", insurance.PolicyholderId);
            }
            ViewData["PolicyholderId"] = new SelectList(_context.Policyholders, "Id", "Id", insurance.PolicyholderId);
            return View(insurance);
        }

        // GET: Insurances/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _context.Insurances
                .Include(i => i.Policyholder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insurance == null)
            {
                return NotFound();
            }
            return View(insurance);
        }

        // POST: Insurances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insurance = await _context.Insurances.FindAsync(id);
            if (insurance == null)
            {
                return NotFound();              
            }
            _context.Insurances.Remove(insurance);
            await _context.SaveChangesAsync();
            return RedirectTo("Details", "Policyholders", insurance.PolicyholderId);
        }

        private bool InsuranceExists(int id)
        {
            return _context.Insurances.Any(e => e.Id == id);
        }

        public IActionResult RedirectTo(string action, string controller, int id)
        {
            return RedirectToAction(action, controller, new { id });
        }
    }
}
