using Microsoft.AspNetCore.Mvc;
using Knapsak_CFTW.Models;
using Microsoft.EntityFrameworkCore;

namespace Knapsak_CFTW.Controllers
{
    public class ItemsController : BaseController
    {
        public ItemsController(ApplicationDbContext db, SessionService session) : base(db, session){}

        public ActionResult Index(List<string> types)
        {
            var items = DB.Items
                .Include(p => p.Evaluations)
                .OrderBy(p => p.Nom)
                .AsQueryable();
            if (types != null && types.Count > 0)
            {
                items = items.Where(i => types.Contains(i.TypeItem));
            }
            ViewBag.SelectedTypes = types;
            return View(items.ToList());
        }

        [Route("Items/DetailsPartial/{id}")]
        public ActionResult DetailsPartial(int id)
        {
            var item = DB.Items
                .Include(i => i.Evaluations)
                    .ThenInclude(e => e.IdJoueurNavigation)
                .Include(i => i.Nourriture)
                .Include(i => i.Medicament)
                .Include(i => i.Armure)
                .Include(i => i.Armes)
                .Include(i => i.Munition)
                .FirstOrDefault(i => i.IdItems == id);

            if (item == null){
                TempData["message"] = "Une erreur est survenue.";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index");
            }

            //item.LienImage = ValiderLienImage(item.LienImage);

            return PartialView("Partials/Details_", item);
        }

        public async Task<ActionResult> Search(string query)
        {
            var items = await DB.Items
                .Where(i => i.Nom.Contains(query))
                .Select(i => new { i.IdItems, i.Nom, i.PrixUnitaire, i.LienImage })
                .OrderBy(i => i.Nom)
                .ToListAsync();

            return Json(items);
        }

        public ActionResult Evaluations(int id)
        {
            var item = DB.Items
                .Where(p => p.IdItems == id)
                .Include(p => p.Nourriture)
                .Include(p => p.Medicament)
                .Include(p => p.Munition)
                .Include(p => p.Armure)
                .Include(p => p.Armes)
                .Include(p => p.Evaluations)
                    .ThenInclude(e => e.IdJoueurNavigation)
                .FirstOrDefault();
            if (item == null)
            {
                TempData["message"] = "Un problèmes est survenu!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "Items");
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEvaluation(int IdItem, short NbEtoiles, string Commentaire)
        {
            try
            {
                var joueur = Session.GetConnected();

                // Vérifier si l'item est dans l'inventaire du joueur
                var itemInInventory = DB.SacADos
                    .Where(s => s.IdJoueur == joueur.IdJoueurs && s.IdItem == IdItem)
                    .FirstOrDefault();

                if (itemInInventory == null)
                {
                    TempData["message"] = "Vous devez posséder cet item dans votre inventaire pour ajouter une évaluation.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Evaluations", new { id = IdItem });
                }

                var alreadyExists = DB.Evaluations.Any(e => e.IdItem == IdItem && e.IdJoueur == joueur.IdJoueurs);
                if (alreadyExists)
                {
                    TempData["message"] = "Vous avez déjà évalué cet item.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Evaluations", new { id = IdItem });
                }

                var evaluation = new Evaluation
                {
                    IdItem = IdItem,
                    IdJoueur = joueur.IdJoueurs,
                    NbEtoiles = NbEtoiles,
                    Commentaire = (!string.IsNullOrEmpty(Commentaire) ? Commentaire : null)
                };

                DB.Evaluations.Add(evaluation);
                DB.SaveChanges();

                TempData["message"] = "Évaluation ajoutée avec succès.";
                return RedirectToAction("Evaluations", new { id = IdItem });
            }
            catch (NotConnectedException)
            {
                TempData["message"] = "Vous devez être connecté pour soumettre une évaluation.";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Evaluations", new { id = IdItem });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEvaluation(int IdItem, short NbEtoiles, string Commentaire)
        {
            try
            {
                var joueur = Session.GetConnected();

                var eval = DB.Evaluations.FirstOrDefault(e => e.IdItem == IdItem && e.IdJoueur == joueur.IdJoueurs);
                if (eval == null)
                {
                    TempData["message"] = "Évaluation introuvable.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Evaluations", new { id = IdItem });
                }

                eval.NbEtoiles = NbEtoiles;
                eval.Commentaire = Commentaire;

                DB.SaveChanges();

                TempData["message"] = "Évaluation mise à jour.";
                return RedirectToAction("Evaluations", new { id = IdItem });
            }
            catch (NotConnectedException)
            {
                TempData["message"] = "Vous devez être connecté pour modifier une évaluation.";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Evaluations", new { id = IdItem });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteEvaluation(int IdItem, int IdJoueur)
        {
            try
            {
                var joueur = Session.GetConnected();
                Evaluation? eval = null;
                if(joueur.EstAdmin == 1) {
                     eval = DB.Evaluations.FirstOrDefault(e => e.IdItem == IdItem && e.IdJoueur == IdJoueur);
                } else if (IdJoueur == joueur.IdJoueurs)
                {
                     eval = DB.Evaluations.FirstOrDefault(e => e.IdItem == IdItem && e.IdJoueur == joueur.IdJoueurs);
                }
                 
                if (eval == null)
                {
                    TempData["message"] = "Vous ne pouvez pas supprimer cette évaluation.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Evaluations", new { id = IdItem });
                }

                DB.Evaluations.Remove(eval);
                DB.SaveChanges();

                TempData["message"] = "Évaluation supprimée.";
                return RedirectToAction("Evaluations", new { id = IdItem });
            }
            catch (NotConnectedException)
            {
                TempData["message"] = "Vous devez être connecté pour supprimer une évaluation.";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Evaluations", new { id = IdItem });
            }
        }
    }
}
