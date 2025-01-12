using Forum.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Forum.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var notices = db.Notices.OrderByDescending(notice => notice.CreatedAt).Take(3).ToList();
            var threads = db.Threads.ToList();
            var categories = db.Categories.ToList();
            var numberOfUsers = db.Users.Count();
            var numberOfThreads = db.Threads.Count();
            var numberOfMessages = db.Messages.Count();
            var threadStatistics = db.Threads.Select(t => new
            {
                ThreadId = t.Id,
                ViewsCount = t.Views,
                RepliesCount = t.Messages.Count()
            }).ToList();

            ViewBag.Notices = notices;
            ViewBag.NumberOfUsers = numberOfUsers;
            ViewBag.NumberOfThreads = numberOfThreads;
            ViewBag.NumberOfMessages = numberOfMessages;
            ViewBag.Categories = categories;
            ViewBag.ThreadStatistics = threadStatistics;
            return View(threads);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}