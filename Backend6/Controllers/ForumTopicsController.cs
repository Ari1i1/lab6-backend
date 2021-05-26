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
using Microsoft.AspNetCore.Identity;
using Backend6.Services;
using Microsoft.AspNetCore.Authorization;

namespace Backend6.Controllers
{
    [Authorize]
    public class ForumTopicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserPermissionsService _userPermissionsService;

        public ForumTopicsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserPermissionsService userPermissionsService)
        {
            _context = context;
            this._userManager = userManager;
            this._userPermissionsService = userPermissionsService;

        }

        // GET: ForumTopics
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ForumTopics
                .Include(f => f.Creator)
                .Include(f => f.Forum);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ForumTopics/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics
                .Include(f => f.Creator)
                .Include(f => f.Forum)
                .Include(f => f.ForumMessages)
                .ThenInclude(f => f.Creator)
                .Include(f => f.ForumMessages)
                .ThenInclude(f => f.ForumMessageAttachments)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumTopic == null)
            {
                return NotFound();
            }

            return View(forumTopic);
        }

        // GET: ForumTopics/Create
        public async Task<IActionResult> Create(Guid? forumId)
        {
            if (forumId == null)
            {
                return this.NotFound();
            }

            var forum = await this._context.Forums
               .SingleOrDefaultAsync(x => x.Id == forumId);

            if (forum == null)
            {
                return this.NotFound();
            }
            this.ViewBag.Forum = forum;
            return View(new ForumTopicCreateModel());
        }

        // POST: ForumTopics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid? forumId, ForumTopicCreateModel model)
        {
            if (forumId == null)
            {
                return NotFound();
            }

            var forum = await this._context.Forums
               .SingleOrDefaultAsync(x => x.Id == forumId);

            if (forum == null)
            {
                return NotFound();
            }

            var user = await this._userManager.GetUserAsync(this.HttpContext.User);

            if (ModelState.IsValid)
            {
                var forumTopic = new ForumTopic
                {
                    ForumId = forum.Id,
                    CreatorId = user.Id,
                    Created = DateTime.UtcNow,
                    Name = model.Name
                };

                _context.Add(forumTopic);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Forums", new { id = forum.Id });
            }

            ViewBag.Forum = forum;
            return View(model);
        }

        // GET: ForumTopics/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics
                .Include(m => m.Forum)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (forumTopic == null || !this._userPermissionsService.CanEditForumTopic(forumTopic))
            {
                return NotFound();
            }

            var model = new ForumTopicEditModel
            {
                Name = forumTopic.Name
            };

            ViewBag.Forum = forumTopic.Forum;
            return View(model);
        }

        // POST: ForumTopics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? id, ForumTopicEditModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics
                .Include(m => m.Forum)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (forumTopic == null || !this._userPermissionsService.CanEditForumTopic(forumTopic))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                forumTopic.Name = model.Name;

                await this._context.SaveChangesAsync();
                return RedirectToAction("Details", "Forums", new { id = forumTopic.ForumId });
            }

            ViewBag.Forum = forumTopic.Forum;
            return View(model);
        }

        // GET: ForumTopics/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics
                .Include(f => f.Creator)
                .Include(f => f.Forum)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumTopic == null)
            {
                return NotFound();
            }
            ViewBag.Forum = forumTopic.Forum;
            return View(forumTopic);
        }

        // POST: ForumTopics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics
                .Include(f => f.Creator)
                .Include(f => f.Forum)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumTopic == null || !this._userPermissionsService.CanEditForumTopic(forumTopic))
            {
                return NotFound();
            }
            ViewBag.Forum = forumTopic.Forum;
            _context.ForumTopics.Remove(forumTopic);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Forums", new { id = forumTopic.ForumId });
        }
    }
}
