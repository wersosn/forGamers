using Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Forum.Controllers
{
    public class AdminPanelController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager userManager;

        // Przechodzenie do panelu admina:
        public ActionResult AdminPanel()
        {
            if (!IsUserAdmin())
            {
                return new HttpUnauthorizedResult(); // Jeśli nie jest adminem, zwróć błąd 401
            }
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
            if (!IsUserAdmin())
            {
                return new HttpUnauthorizedResult(); // Jeśli nie jest adminem, zwróć błąd 401
            }
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

        private bool IsUserAdmin()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            return user != null && user.Role == "admin";
        }


    }




}