using Knapsak_CFTW.Models;
using Microsoft.AspNetCore.Mvc;

namespace Knapsak_CFTW.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext DB;
        protected readonly SessionService Session;
        private static readonly string[] ValidImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg" };
        private static readonly string DefaultImageUrl = "/images/default.png";

        public BaseController(ApplicationDbContext db, SessionService session)
        {
            DB = db;
            Session = session;
        }
        protected string ValiderLienImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return DefaultImageUrl;

            var extension = Path.GetExtension(url).ToLower();

            if (!ValidImageExtensions.Contains(extension))
                return DefaultImageUrl;

            return url;
        }
        protected ActionResult AdminOnly(Func<ActionResult> action)
        {
            try
            {
                var joueur = Session.GetConnected();
                if (joueur.EstAdmin != 1)
                {
                    TempData["message"] = "Vous n'avez pas les permissions pour aller ici!";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("ForceSignOutRedirect", "Accounts");
                }
                return action();
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("ForceSignOutRedirect", "Accounts");
            }
        }   
    }
}
