using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend6.Data;
using Backend6.Models;
using Microsoft.AspNetCore.Identity;
using Backend6.Services;
using Backend6.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Backend6.Controllers
{
    [Authorize]
    public class ForumMessagesController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserPermissionsService userPermissionsService;

        public ForumMessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserPermissionsService userPermissions)
        {
            _context = context;
            this.userManager = userManager;
            this.userPermissionsService = userPermissions;

        }

        // GET: ForumMessages
        public async Task<IActionResult> Index(Guid forumTopicId)
        {
            if (forumTopicId == null)
            {
                return this.NotFound();
            }

            var forumTopic = await this._context.ForumTopics
                 .SingleOrDefaultAsync(x => x.Id == forumTopicId);

            if (forumTopic == null)
            {
                return this.NotFound();
            }
            this.ViewBag.ForumTopic = forumTopic;

            var forumMessages = await this._context.ForumMessages
                .Include(w => w.ForumTopic)
                .Include(w => w.Creator)
                .Where(x => x.ForumTopicId == forumTopicId)
                .ToListAsync();

            return this.View(forumMessages);
        }

        // GET: ForumMessages/Create
        public async Task<IActionResult> Create(Guid? forumTopicId)
        {
            if (forumTopicId == null)
            {
                return this.NotFound();
            }

            var forumTopic = await this._context.ForumTopics
                .SingleOrDefaultAsync(m => m.Id == forumTopicId);

            if (forumTopic == null)
            {
                return this.NotFound();
            }

            this.ViewBag.ForumTopic = forumTopic;
            return View(new ForumMessageCreateModel());
        }

        // POST: ForumMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid forumTopicId, ForumMessageCreateModel model)
        {
            if (forumTopicId == null)
            {
                return this.NotFound();
            }

            var forumTopic = await this._context.ForumTopics
                .SingleOrDefaultAsync(m => m.Id == forumTopicId);

            if (forumTopic == null)
            {
                return this.NotFound();
            }

            var user = await this.userManager.GetUserAsync(this.HttpContext.User);

            if (ModelState.IsValid)
            {
                var forumMessage = new ForumMessage
                {
                    ForumTopicId = forumTopic.Id,
                    CreatorId = user.Id,
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow,
                    Text = model.Text
                };
                _context.Add(forumMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "ForumTopics", new { id = forumTopic.Id });
            }
            ViewBag.ForumTopic = forumTopic;
            return View(model);
        }

        // GET: ForumMessages/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumMessage = await _context.ForumMessages
                .Include(f => f.ForumTopic)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumMessage == null || !this.userPermissionsService.CanEditForumMessage(forumMessage))
            {
                return NotFound();
            }

            var model = new ForumMessageEditModel
            {
                Text = forumMessage.Text
            };
            this.ViewBag.ForumTopic = forumMessage.ForumTopic;
            return View(model);
        }

        // POST: ForumMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? id, ForumMessageEditModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumMessage = await _context.ForumMessages
                .Include(f => f.ForumTopic)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumMessage == null || !this.userPermissionsService.CanEditForumMessage(forumMessage))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                forumMessage.Text = model.Text;
                forumMessage.Modified = DateTime.UtcNow;

                await this._context.SaveChangesAsync();
                return RedirectToAction("Details", "ForumTopics", new { id = forumMessage.ForumTopicId });
            }
            this.ViewBag.ForumTopic = forumMessage.ForumTopic;
            return View(forumMessage);
        }

        // GET: ForumMessages/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumMessage = await _context.ForumMessages
                .Include(f => f.Creator)
                .Include(f => f.ForumTopic)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumMessage == null)
            {
                return NotFound();
            }
            this.ViewBag.ForumTopic = forumMessage.ForumTopic;
            return View(forumMessage);
        }

        // POST: ForumMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumMessage = await _context.ForumMessages
                .Include(f => f.Creator)
                .Include(f => f.ForumTopic)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumMessage == null)
            {
                return NotFound();
            }
            this.ViewBag.Topic = forumMessage.ForumTopic;
            _context.ForumMessages.Remove(forumMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "ForumTopics", new { id = forumMessage.ForumTopicId });
        }
    }
}
