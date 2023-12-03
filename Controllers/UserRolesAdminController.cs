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
    public class UserRolesAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserRolesAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserRolesAdmin
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserRole.ToListAsync());
        }

        // GET: UserRolesAdmin/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRole
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userRole == null)
            {
                return NotFound();
            }

            return View(userRole);
        }

        // GET: UserRolesAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserRolesAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserName,IsSelected,RoleId")] UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userRole);
        }

        // GET: UserRolesAdmin/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRole.FindAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }
            return View(userRole);
        }

        // POST: UserRolesAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,UserName,IsSelected,RoleId")] UserRole userRole)
        {
            if (id != userRole.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserRoleExists(userRole.UserId))
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
            return View(userRole);
        }

        // GET: UserRolesAdmin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRole
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userRole == null)
            {
                return NotFound();
            }

            return View(userRole);
        }

        // POST: UserRolesAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userRole = await _context.UserRole.FindAsync(id);
            _context.UserRole.Remove(userRole);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserRoleExists(string id)
        {
            return _context.UserRole.Any(e => e.UserId == id);
        }
    }
}
