using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Knapsak_CFTW.Models;
using System.Security.Cryptography;
using System.Numerics;
using Knapsak_CFTW.ViewModels;

namespace Knapsak_CFTW.Controllers
{
    public class AccountsController : Controller
    {
        protected readonly ApplicationDbContext DB;
        private readonly SessionService Session;
        public AccountsController(ApplicationDbContext db, SessionService session)
        {
            DB = db;
            Session = session;
        }
        static int saltSize = 20;
        public ActionResult MonProfil()
        {
            try
            { 
                Joueur joueur = Session.GetConnected();
                return View(joueur);
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn");
            }
        }
        public ActionResult SignUp()
        {
            return View(new Joueur());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(Joueur joueur)
        {
            if (ModelState.IsValid)
            {
                joueur.MPasse = HashPassword(joueur.MPasse);
                DB.Joueurs.Add(joueur);
                DB.SaveChanges();
                return RedirectToAction("SignIn");
            }
            return View(joueur);
        }

        public ActionResult SignIn()
        {
            return View(new ConnexionViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult SignIn(ConnexionViewModel connexion)
        {
            var joueur = DB.Joueurs.FirstOrDefault(u => u.Alias == connexion.Alias);
            if (joueur != null && ModelState.IsValid && VerifyPassword(connexion.MPasse, joueur.MPasse))
            {
                joueur.PoidsTotal = (short)Session.GetCurrentWeight(joueur.IdJoueurs);
                joueur.NbItemsPanier = Session.GetNbItemsPanier(joueur.IdJoueurs);

                Session.SetSessionId(joueur.IdJoueurs);
                return RedirectToAction("Index", "Items");
            }
            if (joueur != null && joueur.MPasse != connexion.MPasse && connexion.MPasse != null)
            {
                ModelState.AddModelError("CredentialMismatch", "Alias ou mot de passe incorrect.");

            }
            return View(connexion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            Session.ClearSession();
            return RedirectToAction("Index", "Items");
        }
        public ActionResult ForceSignOutRedirect()
        {
            var j = Session.GetConnected(false);
            if (j != null)
                SignOut();
            return View("ForceRedirectToSignIn");
        }
        [HttpPost]
        public JsonResult AliasExist(string alias)
        {
            return Json(!DB.Joueurs.Any(u => u.Alias == alias));
        }

        [HttpPost]
        public JsonResult AliasNotExist(string alias)
        {
            return Json(DB.Joueurs.Any(u => u.Alias == alias));
        }

        [HttpPost]
        public bool AliasAvailable(string alias)
        {
            bool conflict = false;
            Joueur connectedPlayer;
            try
            {
                connectedPlayer = Session.GetConnected();
            }
            catch (NotConnectedException)
            {
                connectedPlayer = null;
            }
            int currentId = connectedPlayer != null ? connectedPlayer.IdJoueurs : 0;
            Joueur? foundUser = DB.Joueurs.ToList().FirstOrDefault(u => u.Alias == alias && u.IdJoueurs != currentId);
            conflict = foundUser != null;
            return !conflict;
        }

        public ActionResult EditProfil()
        {
            Joueur connectedPlayer = Session.GetConnected();
            if (connectedPlayer != null)
            {
                return View("ProfilForm",connectedPlayer);
            }
            return RedirectToAction("SignIn", "Accounts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfil(Joueur joueur)
        {
            Joueur connectedPlayer = Session.GetConnected();
            connectedPlayer.Alias = joueur.Alias;
            if (!string.IsNullOrWhiteSpace(joueur.MPasse) && joueur.MPasse != "********")
            {
                joueur.MPasse = HashPassword(joueur.MPasse);
                connectedPlayer.MPasse = joueur.MPasse;
            }
            DB.Joueurs.Update(connectedPlayer);
            DB.SaveChanges();
            Session.SetSessionId(connectedPlayer.IdJoueurs);
            TempData["message"] = "Le profil a été modifié!";
            TempData["isMessageBad"] = false;
            return RedirectToAction("Index", "Items");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendRequete()
        {
            try
            {
                var joueur = Session.GetConnected();

                int nbRequetesExistantes = DB.Requetes
                    .Where(r => r.IdJoueurs == joueur.IdJoueurs)
                    .Count();

                if (nbRequetesExistantes >= Joueur.RequeteAmount)
                {
                    TempData["message"] = "Vous avez déjà effectué toutes vos requêtes!";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("MonProfil");
                }

                int capsules = 0;
                switch (nbRequetesExistantes)
                {
                    case 0:
                        capsules = 300;
                        break;
                    case 1:
                        capsules = 200;
                        break;
                    case 2:
                        capsules = 100;
                        break;
                }

                var nouvelleRequete = new Requetes
                {
                    IdJoueurs = joueur.IdJoueurs,
                    CapsulesDemandes = capsules,
                    EstReglee = false
                };

                DB.Requetes.Add(nouvelleRequete);
                DB.SaveChanges();

                TempData["message"] = "Votre requête a été envoyée avec succès!";
                TempData["isMessageBad"] = false;
                return RedirectToAction("MonProfil");
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn");
            }
        }

        private static string HashPassword(string password, string salt = "")
        {
            if (string.IsNullOrEmpty(salt)) salt = CreateSalt(saltSize);
            string saltedPassword = password + salt;
            HashAlgorithm encryptAlgorithm = new SHA256CryptoServiceProvider();
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(saltedPassword);
            byte[] bytHash = encryptAlgorithm.ComputeHash(bytValue);
            string base64 = Convert.ToBase64String(bytHash);
            return base64 + salt;
        }

        private static bool VerifyPassword(string password, string storedPassword)
        {
            string salt = storedPassword.Substring(storedPassword.Length - CreateSalt(saltSize).Length);
            string hashedPassword = HashPassword(password, salt);
            return hashedPassword == storedPassword;
        }

        private static string CreateSalt(int size)
        {
            RNGCryptoServiceProvider randomNumberGenerator = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            randomNumberGenerator.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }
    }
}