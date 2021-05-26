using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend6.Data;
using Backend6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Backend6.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Backend6.Models.ViewModels;
using Microsoft.Net.Http.Headers;

namespace Backend6.Controllers
{
    [Authorize]
    public class ForumMessageAttachmentsController : Controller
    {
        private static readonly HashSet<String> AllowedExtensions = new HashSet<String> { ".jpg", ".jpeg", ".png", ".gif" };

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserPermissionsService userPermissionsService;
        private readonly IHostingEnvironment hostingEnvironment;

        public ForumMessageAttachmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserPermissionsService userPermissions, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.userManager = userManager;
            this.userPermissionsService = userPermissions;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: ForumMessageAttachments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ForumMessageAttachments.Include(f => f.ForumMessage);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ForumMessageAttachments/Create
        public async Task<IActionResult> Create(Guid? messageId)
        {
            if (messageId == null)
            {
                return this.NotFound();
            }

            var message = await _context.ForumMessages
                .SingleOrDefaultAsync(m => m.Id == messageId);

            if (message == null || !this.userPermissionsService.CanEditForumMessage(message))
            {
                return this.NotFound();
            }

            ViewBag.Message = message;
            return View(new ForumMessageAttachmentCreateModel());
        }

        // POST: ForumMessageAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid? messageId, ForumMessageAttachmentCreateModel model)
        {
            if (messageId == null)
            {
                return this.NotFound();
            }

            var message = await _context.ForumMessages
                .SingleOrDefaultAsync(m => m.Id == messageId);

            if (message == null || !this.userPermissionsService.CanEditForumMessage(message))
            {
                return this.NotFound();
            }

            var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(model.FilePath.ContentDisposition).FileName.Value.Trim('"'));
            var fileExt = Path.GetExtension(fileName);

            if (!AllowedExtensions.Contains(fileExt))
            {
                this.ModelState.AddModelError(nameof(model.FilePath), "This file type is prohibited");
            }
            if (ModelState.IsValid)
            {
                var messageAttachment = new ForumMessageAttachment
                {
                    ForumMessageId = message.Id,
                    Created = DateTime.UtcNow,
                    FileName = model.FileName
                };

                var attachmentPath = Path.Combine(this.hostingEnvironment.WebRootPath, "attachments", messageAttachment.Id.ToString("N") + fileExt);
                messageAttachment.FilePath = $"/attachments/{messageAttachment.Id:N}{fileExt}";
                using (var fileStream = new FileStream(attachmentPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    await model.FilePath.CopyToAsync(fileStream);
                }

                _context.Add(messageAttachment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "ForumTopics", new { id = message.ForumTopicId });
            }
            ViewBag.Message = message;
            return View(model);
        }


        // GET: ForumMessageAttachments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumMessageAttachment = await _context.ForumMessageAttachments
                .Include(f => f.ForumMessage)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumMessageAttachment == null)
            {
                return NotFound();
            }

            return View(forumMessageAttachment);
        }

        // POST: ForumMessageAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumMessageAttachment = await _context.ForumMessageAttachments
                .Include(f => f.ForumMessage)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumMessageAttachment == null)
            {
                return NotFound();
            }
            var attachmentPath = Path.Combine(this.hostingEnvironment.WebRootPath, "attachments", forumMessageAttachment.Id.ToString("N") + Path.GetExtension(forumMessageAttachment.FilePath));
            System.IO.File.Delete(attachmentPath);
            _context.ForumMessageAttachments.Remove(forumMessageAttachment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "ForumTopics", new { id = forumMessageAttachment.ForumMessage.ForumTopicId });
        }
    }
}
