using Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Forum.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminPanelController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager userManager;

        // Przechodzenie do panelu admina:
        public ActionResult AdminPanel()
        {
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            return View();
        }

        // Pobieranie listy użytkowników, aby wyświetlić ich role
        public async Task<ActionResult> UserList()
        {
            
            // Pobierz wszystkich użytkowników
            var users = db.Users.ToList();

            // Dla każdego użytkownika pobierz jego role
            var userRoles = await Task.WhenAll(users.Select(async user => new
            {
                User = user,
                Roles = await userManager.GetRolesAsync(user.Id) // Używamy await, ponieważ GetRolesAsync jest asynchroniczne
            }).ToList());

            return View(userRoles);
        }

        // Wyświetlanie panelu administratora (lista użytkowników)
        public ActionResult Index()
        {
            var users = db.Users.ToList(); // Pobranie wszystkich użytkowników
            return View(users); // Przekazanie użytkowników do widoku
        }

        // Widok do edytowania użytkownika
        public ActionResult EditUser(string id)
        {          
            var user = db.Users.FirstOrDefault(u => u.Id == id); // Pobranie użytkownika po ID
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user); // Przekazanie użytkownika do widoku
        }

        // Akcja do zapisania edytowanego użytkownika
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified; // Aktualizacja danych użytkownika
                db.SaveChanges(); // Zapisanie zmian w bazie
                return RedirectToAction("Index"); // Powrót do głównego panelu
            }
            return View(user); // W razie błędów, ponowne wyświetlenie formularza
        }

        // Akcja do usunięcia użytkownika
        public ActionResult DeleteUser(string id)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return HttpNotFound();
            }

            db.Users.Remove(user); // Usuwanie użytkownika
            db.SaveChanges();
            return RedirectToAction("Index"); // Powrót do widoku użytkowników
        }

        // Wyświetlanie kategorii
        public ActionResult Categories()
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
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }

        // Wyświetlanie forów w danej kategorii
        public ActionResult Forums(int categoryId)
        {
            var forums = db.Forums.Where(f => f.CategoryId == categoryId).ToList();
            return View(forums);
        }

        // Tworzenie nowego forum
        [HttpGet]
        public ActionResult CreateForum()
        {
            ViewBag.Categories = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult CreateForum(Models.Forum forum)
        {
            if (ModelState.IsValid)
            {
                db.Forums.Add(forum);
                db.SaveChanges();
                return RedirectToAction("Index", new { categoryId = forum.CategoryId });
            }
            ViewBag.Categories = new SelectList(db.Categories, "Id", "Name", forum.CategoryId);
            return View(forum);
        }

        // Edytowanie forum
        [HttpGet]
        public ActionResult EditForum(int id)
        {
            var forum = db.Forums.FirstOrDefault(f => f.Id == id);
            if (forum == null) return HttpNotFound();
            ViewBag.Categories = new SelectList(db.Categories, "Id", "Name", forum.CategoryId);
            return View(forum);
        }

        [HttpPost]
        public ActionResult EditForum(Models.Forum forum)
        {
            if (ModelState.IsValid)
            {
                db.Entry(forum).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { categoryId = forum.CategoryId });
            }
            ViewBag.Categories = new SelectList(db.Categories, "Id", "Name", forum.CategoryId);
            return View(forum);
        }

        // Usuwanie forum
        public ActionResult DeleteForum(int id)
        {
            var forum = db.Forums.FirstOrDefault(f => f.Id == id);
            if (forum == null) return HttpNotFound();

            db.Forums.Remove(forum);
            db.SaveChanges();
            return RedirectToAction("Index", new { categoryId = forum.CategoryId });
        }
    }




}