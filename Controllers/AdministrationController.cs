using Knapsak_CFTW.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Knapsak_CFTW.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Knapsak_CFTW.Controllers
{
    public class AdministrationController : BaseController
    {
        public AdministrationController(ApplicationDbContext db, SessionService session) : base(db, session) { } 
        public ActionResult Index() => AdminOnly(() => View());

        public ActionResult Requetes()
        {
            return AdminOnly(() =>
            {
                var requetes = DB.Requetes
                    .Where(r => !r.EstReglee)
                    .Include(r => r.Joueur)
                    .OrderBy(r => r.EstReglee)
                    .ThenBy(r => r.IdRequete)
                    .ToList();

                return PartialView("Partials/Requetes_", requetes);
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RepondreRequete(int idRequete, bool estAcceptee)
        {
            return AdminOnly(() =>
            {

                var requete = DB.Requetes
                                .Include(r => r.Joueur)
                                .FirstOrDefault(r => r.IdRequete == idRequete);

                if (requete == null)
                {
                    TempData["message"] = "Requête introuvable.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Index", new { section = "Requetes" });
                }

                if (requete.EstReglee)
                {
                    TempData["message"] = "Cette requête est déjà réglée.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Index", new { section = "Requetes" });
                }

                if (estAcceptee)
                {
                    var joueur = DB.Joueurs.FirstOrDefault(j => j.IdJoueurs == requete.IdJoueurs);
                    joueur.Montant += requete.CapsulesDemandes;
                    DB.Joueurs.Update(joueur);
                }

                requete.EstReglee = true;
                DB.Requetes.Update(requete);
                DB.SaveChanges();

                TempData["message"] = estAcceptee ? "Requête acceptée et capsules octroyés." : "Requête refusée.";
                TempData["isMessageBad"] = false;
                return RedirectToAction("Index", new { section = "Requetes" });
            });
        }
        

        public ActionResult Items()
        {
            return AdminOnly(() =>
            {
                var items = DB.Items
                  .OrderBy(i => i.TypeItem)
                    .ThenBy(i => i.Nom)
                  .ToList();

                return PartialView("Partials/Items_", items);
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleItemDispo(int id)
        {
            var item = DB.Items.Find(id);
            if (item == null)
                return NotFound();

            item.FlagDispo = (short)(item.FlagDispo == 1 ? 0 : 1);
            DB.SaveChanges();

            return Ok(new { dispo = item.FlagDispo == 1 });
        }
        [Route("Administration/CreateItemPartial")]
        public ActionResult CreateItemPartial()
        {
            return PartialView("Partials/Forms/Item_", new ItemEditViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateItem(ItemEditViewModel model)
        {
            return AdminOnly(() =>
            {
                if (!ModelState.IsValid)
                {
                    TempData["message"] = "Le formulaire contient des erreurs.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Index", new { section = "Items" });
                }

                var item = new Item
                {
                    Nom = model.Nom,
                    QuantiteStock = model.QuantiteStock,
                    TypeItem = model.TypeItem,
                    Description = model.Description,
                    PrixUnitaire = model.PrixUnitaire,
                    Poids = model.Poids,
                    Utilite = model.Utilite,
                    LienImage = ValiderLienImage(model.LienImage),
                    FlagDispo = model.FlagDispo
                };

                DB.Items.Add(item);
                DB.SaveChanges();

                TempData["message"] = "L'objet a été créé avec succès.";
                TempData["isMessageBad"] = false;

                return RedirectToAction("Index", new { section = "Items" });
            });
        }
        [Route("Administration/EditItemPartial/{id}")]
        public ActionResult EditItemPartial(int id)
        {
            var item = DB.Items.FirstOrDefault(i => i.IdItems == id);
            if (item == null) return NotFound();

            var model = new ItemEditViewModel
            {
                IdItems = item.IdItems,
                Nom = item.Nom,
                QuantiteStock = item.QuantiteStock,
                TypeItem = item.TypeItem,
                Description = item.Description,
                PrixUnitaire = item.PrixUnitaire,
                Poids = item.Poids,
                Utilite = item.Utilite,
                LienImage = item.LienImage,
                FlagDispo = item.FlagDispo,
                AvailableTypes = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Armes", Value = "Armes", Selected = item.TypeItem == "Armes" },
                    new SelectListItem { Text = "Medicaments", Value = "Medicaments", Selected = item.TypeItem == "Medicaments" },
                    new SelectListItem { Text = "Armures", Value = "Armures", Selected = item.TypeItem == "Armures" },
                    new SelectListItem { Text = "Nourritures", Value = "Nourritures", Selected = item.TypeItem == "Nourritures" },
                    new SelectListItem { Text = "Munitions", Value = "Munitions", Selected = item.TypeItem == "Munitions" }
                }
            };

            return PartialView("Partials/Forms/Item_", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditItem(ItemEditViewModel model)
        {
            return AdminOnly(() =>
            {
                if (!ModelState.IsValid)
                {
                    TempData["message"] = "Le formulaire contient des erreurs.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Index", new { section = "Items" });
                }

                var item = DB.Items.FirstOrDefault(i => i.IdItems == model.IdItems);
                if (item == null)
                {
                    TempData["message"] = "Objet introuvable.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Index", new { section = "Items" });
                }

                item.Nom = model.Nom;
                item.QuantiteStock = model.QuantiteStock;
                item.TypeItem = model.TypeItem;
                item.Description = model.Description;
                item.PrixUnitaire = model.PrixUnitaire;
                item.Poids = model.Poids;
                item.Utilite = model.Utilite;
                item.LienImage = ValiderLienImage(model.LienImage);

                DB.Items.Update(item);
                DB.SaveChanges();

                TempData["message"] = "L'objet a été mis à jour.";
                TempData["isMessageBad"] = false;

                return RedirectToAction("Index", new { section = "Items" });
            });
        }


        public ActionResult Enigmes()
        {
            return AdminOnly(() =>
            {
                var enigmes = DB.Enigmes
                    .Include(e => e.DifficulteNavigation)
                    .OrderBy(e => e.Difficulte)
                        .ThenBy(e => e.Question)
                    .ToList();

                return PartialView("Partials/Enigmes_", enigmes);
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleEnigmeActivation(int id)
        {
            var enigme = DB.Enigmes.Find(id);
            if (enigme == null)
                return NotFound();

            enigme.EstActif = (short)(enigme.EstActif == 1 ? 0 : 1);
            DB.SaveChanges();

            return Ok(new { active = enigme.EstActif == 1 });
        }
        [Route("Administration/CreateEnigmePartial")]
        public ActionResult CreateEnigmePartial()
        {
            ViewBag.DifficulteList = DB.Difficultes.ToList();
            return PartialView("Partials/Forms/Enigme_", new EnigmeEditViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEnigme(EnigmeEditViewModel model)
        {
            return AdminOnly(() =>
            {
                if (!ModelState.IsValid)
                {
                    TempData["message"] = "Le formulaire contient des erreurs.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Index", new { section = "Enigmes" });
                }

                var enigme = new Enigme
                {
                    Question = model.Question,
                    Difficulte = model.Difficulte,
                    EstActif = model.EstActif,
                };

                DB.Enigmes.Add(enigme);
                DB.SaveChanges(); // Save to get IdEnigmes for foreign keys

                for (int i = 0; i < model.Reponses.Count; i++)
                {
                    var repVm = model.Reponses[i];
                    if (!string.IsNullOrWhiteSpace(repVm.ReponseText))
                    {
                        var reponse = new Reponse
                        {
                            ReponseText = repVm.ReponseText,
                            EstBonneRep = (short)(i == model.IndexBonneReponse ? 1 : 0),
                            IdEnigmes = enigme.IdEnigmes
                        };
                        DB.Reponses.Add(reponse);
                    }
                }

                DB.SaveChanges();

                // Optionally return JSON if handled dynamically or redirect otherwise
                return RedirectToAction("Index", new { section = "Enigmes" });
            });
        }

        [Route("Administration/EditEnigmePartial/{id}")]
        public ActionResult EditEnigmePartial(int id)
        {
            var enigme = DB.Enigmes
                .Include(e => e.Reponses)
                .Include(e => e.DifficulteNavigation)
                .FirstOrDefault(e => e.IdEnigmes == id);

            if (enigme == null) return NotFound();

            var model = new EnigmeEditViewModel
            {
                IdEnigmes = enigme.IdEnigmes,
                Question = enigme.Question,
                Difficulte = enigme.Difficulte,
                DifficulteNavigation = enigme.DifficulteNavigation,
                IndexBonneReponse = enigme.Reponses.ToList().FindIndex(r => r.EstBonneRep == 1),
                EstActif = enigme.EstActif,
                Reponses = enigme.Reponses.Select(r => new ReponseEditViewModel
                {
                    IdReponses = r.IdReponses,
                    ReponseText = r.ReponseText,
                    EstBonneRep = r.EstBonneRep == 1
                }).ToList()
            };

            ViewBag.DifficulteList = DB.Difficultes.ToList();
            return PartialView("Partials/Forms/Enigme_", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEnigme(EnigmeEditViewModel model)
        {
            return AdminOnly(() =>
            {
                if (!ModelState.IsValid)
                {
                    TempData["message"] = "Le formulaire contient des erreurs.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Index", new { section = "Enigmes" });
                }

                var enigme = DB.Enigmes
                    .Include(e => e.Reponses)
                    .FirstOrDefault(e => e.IdEnigmes == model.IdEnigmes);

                if (enigme == null)
                {
                    TempData["message"] = "Énigme introuvable.";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Index", new { section = "Enigmes" });
                }

                // Update Enigme
                enigme.Question = model.Question;
                enigme.Difficulte = model.Difficulte;
                DB.Enigmes.Update(enigme);

                // Update Reponses
                for (int i = 0; i < model.Reponses.Count; i++)
                {
                    var repVm = model.Reponses[i];
                    var existingRep = enigme.Reponses.FirstOrDefault(r => r.IdReponses == repVm.IdReponses);

                    if (existingRep != null)
                    {
                        existingRep.ReponseText = repVm.ReponseText;
                        existingRep.EstBonneRep = (short)((i == model.IndexBonneReponse)?1:0); // <-- Set correct answer here
                        DB.Reponses.Update(existingRep);
                    }
                }


                DB.SaveChanges();

                TempData["message"] = "L'énigme a été mise à jour.";
                TempData["isMessageBad"] = false;

                return RedirectToAction("Index", new { section = "Enigmes" });
            });
        }
    }
}
