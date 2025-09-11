using Knapsak_CFTW.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Security.Cryptography;

namespace Knapsak_CFTW.Controllers
{
    public class EnigmaController : Controller
    {
        protected readonly ApplicationDbContext DB;
        private readonly SessionService Session;
        public EnigmaController(ApplicationDbContext db, SessionService session)
        {
            DB = db;
            Session = session;
        }
        public IActionResult Index(int idDifficulte = 0)
        {
            var enigmesQuery = DB.Enigmes.Where(e => e.EstActif == 1);

            if (idDifficulte != 0)
            {
                enigmesQuery = enigmesQuery.Where(e => e.Difficulte == idDifficulte);
            }

            var enigmes = enigmesQuery
                .Include(e => e.Reponses)
                .Include(e => e.DifficulteNavigation)
                .ToList();

            var random = new Random();
            var enigmeAleatoire = enigmes.Count > 0 ? enigmes[random.Next(enigmes.Count)] : null;

            if (enigmeAleatoire != null)
            {
                Random rnd = new Random(DateTime.Now.Millisecond);

                var shuffledReponses = enigmeAleatoire.Reponses
                    .Select(item => new { item, order = rnd.Next() })
                    .OrderBy(x => x.order)
                    .Select(x => x.item)
                    .ToList();

                enigmeAleatoire.Reponses = shuffledReponses;
            }

            ViewBag.DifficultesSelectionnees = idDifficulte;
            return View(enigmeAleatoire);
        }
        // GET: EnigmaController/Details/5
        [HttpPost]
        public ActionResult Repondre(int idReponse, int idDifficulte)
        {
            try
            {
                var joueur = Session.GetConnected();
                int idEnigme = DB.Reponses.Where(e => e.IdReponses == idReponse).First().IdEnigmes;
                var enigme = DB.Enigmes.Where(e => e.IdEnigmes == idEnigme­)
                    .Include(e => e.DifficulteNavigation)
                    .Include(e => e.Statistiques)
                    .FirstOrDefault();
                if (enigme != null)
                {
                    var reponse = enigme.Reponses.Where(e => e.IdReponses == idReponse).FirstOrDefault();
                    if (reponse != null)
                    {
                        var difficulte = enigme.DifficulteNavigation;
                        var stats = enigme.Statistiques.Where(s => s.IdJoueur == joueur.IdJoueurs).FirstOrDefault();
                        if (stats == null)
                        {
                            DB.Statistiques.Add(new Statistique(joueur.IdJoueurs, enigme.IdEnigmes));
                            DB.SaveChanges();
                            stats = DB.Statistiques.Where(s => s.IdJoueur == joueur.IdJoueurs && s.IdEnigme == enigme.IdEnigmes).First();
                        }
                        if (reponse.EstBonneRep == 1)
                        {
                            int bonus = 0;
                            int currentStreak = HttpContext.Session.GetString("Streak") != null ? int.Parse(HttpContext.Session.GetString("Streak")) : 0;
                            stats.NbReussi++;//changes les statistiques nbreussi++
                            if (difficulte.IdDifficulte == 3)
                                HttpContext.Session.SetString("Streak", (currentStreak + 1).ToString());//augmente le streak si difficile  Session
                            else
                                HttpContext.Session.SetString("Streak", "0");//si pas difficle => streak = 0
                            currentStreak = int.Parse(HttpContext.Session.GetString("Streak"));
                            bonus = currentStreak > 0 && currentStreak % 3 == 0 ? Enigme.StreakBonus : 0;
                            joueur.Montant += (int)difficulte.GainCaps + bonus;//donne caps
                            TempData["message"] = "Bonne réponse! " + (difficulte.IdDifficulte == 3 ? "Série de " + currentStreak : "Série d'enigme difficile réinitialisée");
                            TempData["message"] += (currentStreak > 0 && currentStreak % 3 == 0 ? $" Bonus octroyé ({Enigme.StreakBonus})" :"");
                            TempData["isMessageBad"] = false;
                        }
                        else
                        {
                            joueur.PointsDeVie -= difficulte.PerteVie;//enleve pv
                            joueur.PointsDeVie = (short)(joueur.PointsDeVie < 0 ? 0 : joueur.PointsDeVie);
                            stats.NbRate++;//changes les statistiques nbrate++
                            HttpContext.Session.SetString("Streak", "0");//streak == 0
                            TempData["message"] = "Mauvaise réponse! Série d'enigmes difficiles réinitialisée";
                            TempData["isMessageBad"] = true;
                        }
                        DB.Statistiques.Update(stats);
                        DB.Joueurs.Update(joueur);
                        DB.SaveChanges();
                    }
                }
                return RedirectToAction("Index", new {idDifficulte});
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn", "Accounts");
            }
        }
    }
}
