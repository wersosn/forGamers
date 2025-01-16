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
            var threads = db.Threads.OrderByDescending(t => t.CreatedAt).ToList();
            var pinnedThreads = db.Threads.Where(t => t.isPinned).OrderByDescending(t => t.CreatedAt).Take(3).ToList();
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
            ViewBag.PinnedThreads = pinnedThreads;
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

        public ActionResult ForumThreads(int forumId)
        {
            var forum = db.Forums.FirstOrDefault(f => f.Id == forumId);
            if (forum == null)
            {
                return HttpNotFound();
            }
            var threads = db.Threads.Where(t => t.ForumId == forumId).ToList();
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
            ViewBag.NumberOfUsers = numberOfUsers;
            ViewBag.NumberOfThreads = numberOfThreads;
            ViewBag.NumberOfMessages = numberOfMessages;
            ViewBag.Categories = categories;
            ViewBag.ThreadStatistics = threadStatistics;

            ViewBag.ForumName = forum.Name;
            return View(threads);
        }

        public ActionResult SearchThreads(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Index", "Home");
            }

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

            ViewBag.NumberOfUsers = numberOfUsers;
            ViewBag.NumberOfThreads = numberOfThreads;
            ViewBag.NumberOfMessages = numberOfMessages;
            ViewBag.Categories = categories;
            ViewBag.ThreadStatistics = threadStatistics;

            query = query.Replace("&&", "AND").Replace("||", "OR").Replace("~", "NOT");
            ViewBag.Search = query;

            var keywords = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var threadsQuery = db.Threads.AsQueryable();

            foreach (var keyword in keywords)
            {
                // Obsługa operatorów
                if (keyword.Contains("AND"))
                {
                    var terms = keyword.Split(new[] { "AND" }, StringSplitOptions.None);
                    threadsQuery = threadsQuery.Where(t => terms.All(term => t.Title.ToLower().Contains(term.ToLower()) || t.Content.ToLower().Contains(term.ToLower())));
                }
                else if (keyword.Contains("OR"))
                {
                    var terms = keyword.Split(new[] { "OR" }, StringSplitOptions.None);
                    threadsQuery = threadsQuery.Where(t => terms.Any(term => t.Title.ToLower().Contains(term.ToLower()) || t.Content.ToLower().Contains(term.ToLower())));
                }
                else if (keyword.Contains("NOT"))
                {
                    var terms = keyword.Split(new[] { "NOT" }, StringSplitOptions.None);
                    threadsQuery = threadsQuery.Where(t => !t.Title.ToLower().Contains(terms[1].ToLower()) && !t.Content.ToLower().Contains(terms[1].ToLower()));
                }
                else
                {
                    threadsQuery = threadsQuery.Where(t => t.Title.ToLower().Contains(keyword.ToLower()) || t.Content.ToLower().Contains(keyword.ToLower()));
                }
            }
            var threads = threadsQuery.ToList();
            return View("SearchThreads", threads);
        }
    }
}