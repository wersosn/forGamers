using System;
using System.Linq;
using System.Web.Mvc;
using Forum.Models;
using Microsoft.AspNet.Identity;

namespace Forum.Controllers
{
    public class MessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int threadId, Message message)
        {
            if (ModelState.IsValid)
            {
                // Pobierz bieżącego użytkownika
                var currentUserId = User.Identity.GetUserId();
                var currentUserName = User.Identity.GetUserName();
                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account"); // Jeśli użytkownik nie jest zalogowany, przekieruj do logowania
                }

                // Przypisz właściwości wiadomości
                message.ThreadId = threadId;
                message.UserId = currentUserId;
                message.UserName = currentUserName;
                message.CreatedAt = DateTime.Now;

                // Dodaj wiadomość do bazy danych
                db.Messages.Add(message);
                db.SaveChanges();

                // Przekierowanie do szczegółów wątku
                return RedirectToAction("Details", "Threads", new { id = threadId });
            }
            return RedirectToAction("Index", "Threads"); // W przypadku błędu
        }
    }
}
