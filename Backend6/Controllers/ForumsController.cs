using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend6.Data;
using Backend6.Models;
using Backend6.Models.ViewModels;
using System.Collections.ObjectModel;

namespace Backend6.Controllers
{
    public class ForumsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForumsController(ApplicationDbContext context)
        {
            this._context = context;
        }

        // GET: Forums
        public async Task<IActionResult> Index(Guid forumCategoryId)
        {
           if (forumCategoryId == null)
            {
                return this.NotFound();
            }

            var forumCategory = await this._context.ForumCategories
                 .SingleOrDefaultAsync(x => x.Id == forumCategoryId);

            if (forumCategory == null)
            {
                return this.NotFound();
            }
            this.ViewBag.ForumCategory = forumCategory;

            var forums = await this._context.Forums
                .Include(w => w.ForumCategory)
                .Where(x => x.ForumCategoryId == forumCategoryId)
                .ToListAsync();

            return this.View(forums);
        }

        // GET: Forums/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var forum = await this._context.Forums
                .Include(f => f.ForumCategory)
                .Include(x => x.ForumTopics)
                .ThenInclude(p => p.Creator)
                .Include(f => f.ForumTopics)
                .ThenInclude(m => m.ForumMessages)
                .ThenInclude(p => p.Creator)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forum == null)
            {
                return this.NotFound();
            }
            this.ViewBag.Forum = forum;
            return this.View(forum);
        }

        // GET: Forums/Create
        public async Task<IActionResult> Create(Guid forumCategoryId)
        {
            if (forumCategoryId == null)
            {
                return this.NotFound();
            }

            var forumCategory = await this._context.ForumCategories
                .SingleOrDefaultAsync(x => x.Id == forumCategoryId);

            if (forumCategory == null)
            {
                return this.NotFound();
            }

            this.ViewBag.ForumCategory = forumCategory;
            return this.View(new ForumCreateModel());
        }

        // POST: Forums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid forumCategoryId, ForumCreateModel model)
        {
            if (forumCategoryId == null)
            {
                return this.NotFound();
            }

            var forumCategory = await this._context.ForumCategories
                .SingleOrDefaultAsync(x => x.Id == forumCategoryId);

            if (forumCategory == null)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                var forum = new Forum
                {
                    ForumCategoryId = forumCategory.Id,
                    Name = model.Name,
                    Description = model.Description,
                    ForumTopics = new Collection<ForumTopic>()
                };

                this._context.Add(forum);
                await this._context.SaveChangesAsync();
                return this.RedirectToAction("Index", "ForumCategories", new { forumCategoryId = forumCategory.Id });
            }
            this.ViewBag.ForumCategory = forumCategory;
            return View(model);
        }

        // GET: Forums/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forum = await _context.Forums
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forum == null)
            {
                return NotFound();
            }
            var model = new ForumEditModel
            {
                Name = forum.Name,
                Description = forum.Description
            };

            this.ViewBag.ForumCategoryId = forum.ForumCategoryId;
            return View(model);
        }

        // POST: Forums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ForumEditModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forum = await _context.Forums
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forum == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                forum.Name = model.Name;
                forum.Description = model.Description;
                await this._context.SaveChangesAsync();
                return this.RedirectToAction("Index", "ForumCategories", new { forumCategoryId = forum.ForumCategoryId });
            }
            this.ViewBag.ForumCategoryId = forum.ForumCategoryId;
            return View(model);
        }

        // GET: Forums/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forum = await _context.Forums
                .Include(f => f.ForumCategory)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forum == null)
            {
                return NotFound();
            }
            this.ViewBag.ForumCategoryId = forum.ForumCategoryId;
            return View(forum);
        }

        // POST: Forums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var forum = await _context.Forums
                .SingleOrDefaultAsync(m => m.Id == id);
            _context.Forums.Remove(forum);
            await _context.SaveChangesAsync();
            this.ViewBag.ForumCategoryId = forum.ForumCategoryId;
            return RedirectToAction("Index", "ForumCategories", new { forumCategoryId = forum.ForumCategoryId });
        }
    }
}
