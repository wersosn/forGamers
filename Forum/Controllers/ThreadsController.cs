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

        public ActionResult Details(int id)
        {
            var thread = db.Threads.Include("Messages").Include("Forum").FirstOrDefault(t => t.Id == id);
            var currentUserId = User.Identity.GetUserId();
            var isModerator = db.ForumModerators
                                .Any(fm => fm.ForumId == thread.ForumId && fm.UserId == currentUserId);

            ViewBag.IsModerator = isModerator;
            return View(thread);
        }

    }
}
