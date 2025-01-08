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
            var threads = db.Threads.ToList();
            var numberOfUsers = db.Users.Count();
            var numberOfThreads = db.Threads.Count();
            var numberOfMessages = db.Messages.Count();
            ViewBag.NumberOfUsers = numberOfUsers;
            ViewBag.NumberOfThreads = numberOfThreads;
            ViewBag.NumberOfMessages = numberOfMessages;
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

        /*private bool IsUserAdmin()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            return user != null && user.Role == "admin";
        }*/
    }
}