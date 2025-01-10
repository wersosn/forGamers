using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using Forum.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System.Data.Entity;


namespace Forum.Controllers
{
    public class MessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // POST: Tworzenie wiadomości
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int threadId, Message message)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity.GetUserId();
                var currentUserName = User.Identity.GetUserName();
                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                message.ThreadId = threadId;
                message.UserId = currentUserId;
                message.UserName = currentUserName;
                message.CreatedAt = DateTime.Now;
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Details", "Threads", new { id = threadId });
            }
            return RedirectToAction("Details", "Threads");
        }

        // GET: Edycja wiadomości
        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            var message = db.Messages
                    .Include(m => m.Thread)
                    .Include(m => m.Thread.Forum)
                    .FirstOrDefault(m => m.Id == id);
            if (message == null)
            {
                return HttpNotFound();
            }

            var forum = message.Thread.Forum;
            var currentUserId = User.Identity.GetUserId();
            bool isModerator = db.ForumModerators.Any(fm => fm.ForumId == forum.Id && fm.UserId == currentUserId);
            if (!(isModerator || User.IsInRole("admin")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Nie masz uprawnień do edycji tej wiadomości.");
            }

            return View(message);
        }

        // POST: Edycja wiadomości
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Message editedMessage)
        {
            if (ModelState.IsValid)
            {
                var message = db.Messages
                    .Include(m => m.Thread)
                    .Include(m => m.Thread.Forum)
                    .FirstOrDefault(m => m.Id == editedMessage.Id);
                if (message == null)
                {
                    return HttpNotFound();
                }

                var forum = message.Thread.Forum;
                var currentUserId = User.Identity.GetUserId();
                bool isModerator = db.ForumModerators.Any(fm => fm.ForumId == forum.Id && fm.UserId == currentUserId);
                if (!(isModerator || User.IsInRole("admin")))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Nie masz uprawnień do edycji tej wiadomości.");
                }

                message.Content = editedMessage.Content;
                db.SaveChanges();

                return RedirectToAction("Details", "Threads", new { id = message.ThreadId });
            }

            return View(editedMessage);
        }

        // GET: Usuwanie wiadomości
        [HttpGet]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var message = db.Messages
                .Include(m => m.Thread)
                .Include(m => m.Thread.Forum)
                .FirstOrDefault(m => m.Id == id);
            if (message == null)
            {
                return HttpNotFound();
            }

            var forum = message.Thread.Forum;
            var currentUserId = User.Identity.GetUserId();
            bool isModerator = db.ForumModerators.Any(fm => fm.ForumId == forum.Id && fm.UserId == currentUserId);
            if (!(isModerator || User.IsInRole("admin")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Nie masz uprawnień do usunięcia tej wiadomości.");
            }

            return View(message);
        }

        // POST: Usuwanie wiadomości
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var message = db.Messages
                .Include(m => m.Thread)
                .Include(m => m.Thread.Forum)
                .FirstOrDefault(m => m.Id == id);
            if (message == null)
            {
                return HttpNotFound();
            }

            var forum = message.Thread.Forum;
            var currentUserId = User.Identity.GetUserId();
            bool isModerator = db.ForumModerators.Any(fm => fm.ForumId == forum.Id && fm.UserId == currentUserId);
            if (!(isModerator || User.IsInRole("admin")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Nie masz uprawnień do usunięcia tej wiadomości.");
            }

            db.Messages.Remove(message);
            db.SaveChanges();

            return RedirectToAction("Details", "Threads", new { id = message.ThreadId });
        }
    }
}
