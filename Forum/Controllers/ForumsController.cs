using Forum.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Forum.Controllers
{
    [Authorize(Roles = "admin")]
    public class ForumsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Forums
        public ActionResult Index()
        {
            return View();
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
                return RedirectToAction("Index", "AdminPanel");
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
                return RedirectToAction("Index", "AdminPanel");
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
            return RedirectToAction("Index", "AdminPanel");
        }

        [HttpGet]
        public ActionResult ManageModerators(int forumId)
        {
            var forum = db.Forums.Include("Moderators.User").FirstOrDefault(f => f.Id == forumId);
            if (forum == null)
                return HttpNotFound();

            var allUsers = db.Users.ToList();
            var viewModel = new ManageModerators
            {
                Forum = forum,
                AllUsers = allUsers,
                AssignedModeratorIds = forum.Moderators.Select(m => m.UserId).ToList()
            };

            return View(viewModel);
        }

        // Zarządzanie moderatorami danego forum
        [HttpPost]
        public ActionResult ManageModerators(int forumId, List<string> selectedModerators)
        {
            var forum = db.Forums.Include("Moderators").FirstOrDefault(f => f.Id == forumId);
            if (forum == null)
                return HttpNotFound();

            forum.Moderators.Clear();
            if (selectedModerators != null)
            {
                foreach (var userId in selectedModerators)
                {
                    forum.Moderators.Add(new ForumModerator { ForumId = forumId, UserId = userId });
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index", "AdminPanel");
        }

    }
}