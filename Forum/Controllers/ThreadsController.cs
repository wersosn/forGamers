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
            if (ModelState.IsValid)
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

        public ActionResult Index()
        {
            var threads = db.Threads.Include("Category").ToList(); // Pobierz wszystkie wątki z kategoriami
            return View(threads);
        }

        public ActionResult Details(int id)
        {
            var thread = db.Threads.Include("Messages").FirstOrDefault(t => t.Id == id);
            return View(thread);
        }
    }
}
