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

        public ActionResult Index()
        {
            var users = db.Users.ToList();
            var threads = db.Threads.ToList();
            var messages = db.Messages.ToList();
            var forums = db.Forums.ToList();
            var categories = db.Categories.ToList();
            var roles = db.Roles.Select(r => new { r.Id, r.Name }).ToList();
            ViewBag.Roles = roles;
            ViewBag.Forums = forums;
            ViewBag.Categories = categories;
            ViewBag.Threads = threads;
            ViewBag.Messages = messages;
            return View(users);
        }

        public async Task<ActionResult> UserList()
        {
            var users = db.Users.ToList();
            var userRoles = await Task.WhenAll(users.Select(async user => new
            {
                User = user,
                Roles = await userManager.GetRolesAsync(user.Id)
            }).ToList());
            var userRoleNames = userRoles.Select(userRole => new
            {
                userRole.User,
                RoleNames = userRole.Roles.Select(role => db.Roles.FirstOrDefault(r => r.Name == role)?.Name ?? "Brak roli").ToList()
            }).ToList();
            return View(userRoleNames);
        }

        // Widok do edytowania użytkownika
        public ActionResult EditUser(string id)
        {          
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var roles = db.Roles.Select(r => new { r.Id, r.Name }).ToList();
            var userRoles = user.Roles.Select(ur => ur.RoleId).ToList();
            ViewBag.Roles = new SelectList(roles, "Id", "Name", userRoles);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(string id, string[] SelectedRoles)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var currentRoles = userManager.GetRoles(user.Id);
            userManager.RemoveFromRoles(user.Id, currentRoles.ToArray());
            if (SelectedRoles != null && SelectedRoles.Any())
            {
                userManager.AddToRoles(user.Id, SelectedRoles);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Widok do usunięcia użytkownika
        public ActionResult DeleteUser(string id)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return HttpNotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Widok do zarządzania słownikiem słów zakazanych
        public ActionResult ForbiddenWords()
        {
            var words = db.ForbiddenWords.ToList();
            return View(words);
        }

        // Dodawanie słów zakazanych
        [HttpGet]
        public ActionResult AddForbiddenWord()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddForbiddenWord(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                var forbiddenWord = new ForbiddenWord { Word = word };
                db.ForbiddenWords.Add(forbiddenWord);
                db.SaveChanges();
            }
            return RedirectToAction("ForbiddenWords");
        }

        // Usuwanie słów zakazanych
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForbiddenWord(int id)
        {
            var word = db.ForbiddenWords.Find(id);
            if (word != null)
            {
                db.ForbiddenWords.Remove(word);
                db.SaveChanges();
            }
            return RedirectToAction("ForbiddenWords");
        }
    }
}