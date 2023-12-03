using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ContractsAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractsAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ContractsAdmin
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Contracts.Include(c => c.Provider).Include(c => c.Requester);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ContractsAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Provider)
                .Include(c => c.Requester)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: ContractsAdmin/Create
        public IActionResult Create()
        {
            ViewData["ProviderId"] = new SelectList(_context.User, "Id", "Id");
            ViewData["RequesterId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: ContractsAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,Title,Description,StartDate,EndDate,Status,Price,Total,ProviderId,RequesterId")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProviderId"] = new SelectList(_context.User, "Id", "Id", contract.ProviderId);
            ViewData["RequesterId"] = new SelectList(_context.User, "Id", "Id", contract.RequesterId);
            return View(contract);
        }

        // GET: ContractsAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["ProviderId"] = new SelectList(_context.User, "Id", "Id", contract.ProviderId);
            ViewData["RequesterId"] = new SelectList(_context.User, "Id", "Id", contract.RequesterId);
            return View(contract);
        }

        // POST: ContractsAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,Title,Description,StartDate,EndDate,Status,Price,Total,ProviderId,RequesterId")] Contract contract)
        {
            if (id != contract.ContractId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.ContractId))
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
            ViewData["ProviderId"] = new SelectList(_context.User, "Id", "Id", contract.ProviderId);
            ViewData["RequesterId"] = new SelectList(_context.User, "Id", "Id", contract.RequesterId);
            return View(contract);
        }

        // GET: ContractsAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Provider)
                .Include(c => c.Requester)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: ContractsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }
    }
}
