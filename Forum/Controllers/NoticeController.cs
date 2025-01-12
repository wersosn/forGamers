using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Forum.Models;
using Microsoft.AspNet.Identity;

namespace Forum.Controllers
{
    public class NoticeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Notice
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Notice notice)
        {
            if (ModelState.IsValid && User.Identity.IsAuthenticated && User.IsInRole("admin"))
            {
                notice.AdminId = User.Identity.GetUserId();
                notice.AdminName = User.Identity.GetUserName();
                db.Notices.Add(notice);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(notice);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            var notice = db.Notices.FirstOrDefault(n => n.Id == id);
            if (notice == null)
            {
                return HttpNotFound();
            }
            return View(notice);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Notice notice)
        {
            if (ModelState.IsValid)
            {
                var existingNotice = db.Notices.FirstOrDefault(n => n.Id == notice.Id);
                if (existingNotice == null)
                {
                    return HttpNotFound();
                }
                existingNotice.Title = notice.Title;
                existingNotice.Content = notice.Content;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(notice);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            var notice = db.Notices.FirstOrDefault(n => n.Id == id);
            if (notice == null)
            {
                return HttpNotFound();
            }
            return View(notice);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var notice = db.Notices.FirstOrDefault(n => n.Id == id);
            if (notice == null)
            {
                return HttpNotFound();
            }
            db.Notices.Remove(notice);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var notice = db.Notices.FirstOrDefault(n => n.Id == id);
            if (notice == null)
            {
                return HttpNotFound();
            }
            return View(notice);
        }
    }
}