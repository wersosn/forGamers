using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Forum.Models;

namespace Forum.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        // Tworzenie nowej kategorii
        [HttpGet]
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index", "AdminPanel");
            }
            return View(category);
        }

        // Edytowanie kategorii
        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            var category = db.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

        [HttpPost]
        public ActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "AdminPanel");
            }
            return View(category);
        }

        // Usuwanie kategorii
        public ActionResult DeleteCategory(int id)
        {
            var category = db.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return HttpNotFound();

            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index", "AdminPanel");
        }
    }
}
