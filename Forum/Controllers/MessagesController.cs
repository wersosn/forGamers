using System.Linq;
using System.Web.Mvc;
using Forum.Models;

namespace Forum.Controllers
{
    public class MessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Message message)
        {
            if (ModelState.IsValid)
            {
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Details", "Threads", new { id = message.ThreadId }); // Przekierowanie do szczegółów wątku
            }
            return RedirectToAction("Index", "Threads"); // W przypadku błędu
        }
    }
}
