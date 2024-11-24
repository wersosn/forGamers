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
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Thread thread, int categoryId)
        {
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                thread.UserId = User.Identity.GetUserId();
                thread.UserName = User.Identity.GetUserName();
                thread.CategoryId = categoryId;
                db.Threads.Add(thread);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Categories = db.Categories.ToList();         
            return View(thread);
        }

        // Wyświetlenie formularza do edycji wątku
        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            if (!IsUserAdmin())
            {
                return new HttpUnauthorizedResult();
            }
            var thread = db.Threads.FirstOrDefault(t => t.Id == id);
            if (thread == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categories = db.Categories.ToList();
            return View(thread);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Thread thread, int categoryId)
        {
            if (ModelState.IsValid)
            {
                var existingThread = db.Threads.FirstOrDefault(t => t.Id == thread.Id);
                if (existingThread != null)
                {
                    existingThread.Title = thread.Title;
                    existingThread.Content = thread.Content;
                    existingThread.CategoryId = categoryId;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Categories = db.Categories.ToList();
            return View(thread);
        }

        // Wyświetlanie formularza do usuwania wątku 
        [HttpGet]
        [Authorize]
        public ActionResult Delete(int id)
        {
            if (!IsUserAdmin())
            {
                return new HttpUnauthorizedResult();
            }
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
            var threads = db.Threads.Include("Category").ToList(); // Pobierz wszystkie wątki z kategoriami
            return View(threads);
        }

        public ActionResult Details(int id)
        {
            var thread = db.Threads.Include("Messages").FirstOrDefault(t => t.Id == id);
            ViewBag.IsAdmin = IsUserAdmin();
            ViewBag.IsMod = IsUserMod();
            return View(thread);
        }

        private bool IsUserAdmin()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            return user != null && user.Role == "admin";
        }
        private bool IsUserMod()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            return user != null && user.Role == "moderator";
        }
    }
}
