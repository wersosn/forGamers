using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
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
                    return RedirectToAction("Login", "Account");
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

        // GET: Edycja wiadomości
        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            var message = db.Messages.FirstOrDefault(m => m.Id == id);

            // Sprawdzenie, czy wiadomość istnieje
            if (message == null)
            {
                return HttpNotFound();
            }

            return View(message);
        }

        // POST: Edycja wiadomości
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Message editedMessage)
        {
            if (ModelState.IsValid)
            {
                var message = db.Messages.FirstOrDefault(m => m.Id == editedMessage.Id);

                // Sprawdzenie, czy wiadomość istnieje
                if (message == null)
                {
                    return HttpNotFound();
                }

                // Aktualizacja treści wiadomości
                message.Content = editedMessage.Content;
                db.SaveChanges();

                return RedirectToAction("Details", "Threads", new { id = message.ThreadId });
            }

            return View(editedMessage);
        }

        // GET: Usuwanie wiadomości
        [HttpGet]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var message = db.Messages.FirstOrDefault(m => m.Id == id);

            // Sprawdzenie, czy wiadomość istnieje
            if (message == null)
            {
                return HttpNotFound();
            }

            return View(message);
        }

        // POST: Usuwanie wiadomości
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var message = db.Messages.FirstOrDefault(m => m.Id == id);

            // Sprawdzenie, czy wiadomość istnieje
            if (message == null)
            {
                return HttpNotFound();
            }

            // Usunięcie wiadomości z bazy danych
            db.Messages.Remove(message);
            db.SaveChanges();

            return RedirectToAction("Details", "Threads", new { id = message.ThreadId });
        }
    }
}
