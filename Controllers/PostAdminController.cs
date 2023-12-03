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
    public class PostAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PostAdmin
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ServicesPosts.Include(s => s.Categories).Include(s => s.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PostAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicesPost = await _context.ServicesPosts
                .Include(s => s.Categories)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.ServicesPostId == id);
            if (servicesPost == null)
            {
                return NotFound();
            }

            return View(servicesPost);
        }

        // GET: PostAdmin/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: PostAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServicesPostId,SearchTag,SearchTagid,Title,Description,CategoryId,UserId")] ServicesPost servicesPost)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicesPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", servicesPost.CategoryId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", servicesPost.UserId);
            return View(servicesPost);
        }

        // GET: PostAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicesPost = await _context.ServicesPosts.FindAsync(id);
            if (servicesPost == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", servicesPost.CategoryId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", servicesPost.UserId);
            return View(servicesPost);
        }

        // POST: PostAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServicesPostId,SearchTag,SearchTagid,Title,Description,CategoryId,UserId")] ServicesPost servicesPost)
        {
            if (id != servicesPost.ServicesPostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicesPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicesPostExists(servicesPost.ServicesPostId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", servicesPost.CategoryId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", servicesPost.UserId);
            return View(servicesPost);
        }

        // GET: PostAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicesPost = await _context.ServicesPosts
                .Include(s => s.Categories)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.ServicesPostId == id);
            if (servicesPost == null)
            {
                return NotFound();
            }

            return View(servicesPost);
        }

        // POST: PostAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicesPost = await _context.ServicesPosts.FindAsync(id);
            _context.ServicesPosts.Remove(servicesPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicesPostExists(int id)
        {
            return _context.ServicesPosts.Any(e => e.ServicesPostId == id);
        }
    }
}
