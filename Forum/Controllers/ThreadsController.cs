using System;
using System.Linq;
using System.Web.Mvc;
using Forum.Models;
using Microsoft.AspNet.Identity;

namespace Forum.Controllers
{
    public class ThreadsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Wyświetlenie formularza do dodawania nowego wątku
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Forums = db.Forums.ToList();
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Thread thread, int forumId)
        {
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                thread.UserId = User.Identity.GetUserId();
                thread.UserName = User.Identity.GetUserName();
                thread.ForumId = forumId;
                db.Threads.Add(thread);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Forums = db.Forums.ToList();         
            return View(thread);
        }

        // Wyświetlenie formularza do edycji wątku
        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            var thread = db.Threads.FirstOrDefault(t => t.Id == id);
            if (thread == null)
            {
                return HttpNotFound();
            }
            ViewBag.Forums = db.Forums.ToList();
            return View(thread);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Thread thread, int forumId)
        {
            if (ModelState.IsValid)
            {
                var existingThread = db.Threads.FirstOrDefault(t => t.Id == thread.Id);
                if (existingThread != null)
                {
                    existingThread.Title = thread.Title;
                    existingThread.Content = thread.Content;
                    existingThread.ForumId = forumId;
                    existingThread.isPinned = thread.isPinned;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Forums = db.Forums.ToList();
            return View(thread);
        }

        // Wyświetlanie formularza do usuwania wątku 
        [HttpGet]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var thread = db.Threads.FirstOrDefault(t => t.Id == id);
            if (thread == null)
            {
                return HttpNotFound();
            }
            return View(thread);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var thread = db.Threads.FirstOrDefault(t => t.Id == id);
            if (thread != null)
            {
                db.Threads.Remove(thread);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            var threads = db.Threads.Include("Forum").ToList();
            return View(threads);
        }

        public ActionResult Details(int id, string searchQuery = null)
        {
            var thread = db.Threads.Include("Messages").Include("Forum").FirstOrDefault(t => t.Id == id);
            db.Database.ExecuteSqlCommand("UPDATE Threads SET Views = Views + 1 WHERE Id = @p0", id);
            var currentUserId = User.Identity.GetUserId();
            var isModerator = db.ForumModerators.Any(fm => fm.ForumId == thread.ForumId && fm.UserId == currentUserId);
            ViewBag.IsModerator = isModerator;
            return View(thread);
        }

        public ActionResult SearchMessages(int threadId, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Details", new { id = threadId });
            }
            var thread = db.Threads.Include("Messages").Include("Forum").FirstOrDefault(t => t.Id == threadId);
            if (thread == null)
            {
                return HttpNotFound();
            }
            var messagesQuery = thread.Messages.AsQueryable();
            query = query.Replace("&&", "AND").Replace("||", "OR").Replace("~", "NOT");
            var keywords = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var keyword in keywords)
            {
                if (keyword.Contains("AND"))
                {
                    var terms = keyword.Split(new[] { "AND" }, StringSplitOptions.None);
                    messagesQuery = messagesQuery.Where(m => terms.All(term => m.Content.ToLower().Contains(term.ToLower())));
                }
                else if (keyword.Contains("OR"))
                {
                    var terms = keyword.Split(new[] { "OR" }, StringSplitOptions.None);
                    messagesQuery = messagesQuery.Where(m => terms.Any(term => m.Content.ToLower().Contains(term.ToLower())));
                }
                else if (keyword.Contains("NOT"))
                {
                    var terms = keyword.Split(new[] { "NOT" }, StringSplitOptions.None);
                    messagesQuery = messagesQuery.Where(m => !m.Content.ToLower().Contains(terms[1].ToLower()));
                }
                else
                {
                    messagesQuery = messagesQuery.Where(m => m.Content.ToLower().Contains(keyword.ToLower()));
                }
            }
            var filteredMessages = messagesQuery.ToList();
            var currentUserId = User.Identity.GetUserId();
            var isModerator = db.ForumModerators.Any(fm => fm.ForumId == thread.ForumId && fm.UserId == currentUserId);
            ViewBag.ThreadDetails = thread;
            ViewBag.Forum = thread.Forum.Name;
            ViewBag.IsModerator = isModerator;
            ViewBag.Search = query;
            ViewBag.SearchResults = filteredMessages;
            return View("SearchMessages");
        }
    }
}
