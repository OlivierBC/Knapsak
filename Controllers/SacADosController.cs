using Knapsak_CFTW.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;

namespace Knapsak_CFTW.Controllers
{
    public class SacADosController : Controller
    {
        protected readonly ApplicationDbContext DB;
        private readonly SessionService Session;
        public SacADosController(ApplicationDbContext db, SessionService session)
        {
            DB = db;
            Session = session;
        }
        public ActionResult Index()
        {
            try
            {
                Joueur j = Session.GetConnected();
                var sac = DB.SacADos
                    .Where(p => p.IdJoueur == j.IdJoueurs)
                    .Include(p => p.IdItemNavigation)
                    .ToList();
                sac.Where(p => p.IdItemNavigation.IsMedicament || p.IdItemNavigation.IsNourriture)
                    .ToList()
                    .ForEach(p => p.gainVie = Session.GetGainVie(p.IdItem));
                return View(sac);
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn", "Accounts");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Consommer(int idItem)
        {
            try
            {
                Joueur joueur = Session.GetConnected();
                var item = DB.SacADos.Include(i => i.IdItemNavigation).FirstOrDefault(s => s.IdJoueur == joueur.IdJoueurs && s.IdItem == idItem);
                if (item != null)
                {
                    var iItem = DB.Items.First(i => i.IdItems == idItem);
                    iItem.CheckIfConsummable();
                    short? gainVie = Session.GetGainVie(iItem);
                    joueur.PointsDeVie += (short)(gainVie != null && gainVie > 0 ? gainVie : 0);

                    if (iItem.IsNourriture)
                    {
                        joueur.Dexterite += 2;
                        TempData["message"] = "L'item a été consommé! Vous avez gagné 2 points de dextérité!";
                    }
                    else
                    {
                        TempData["message"] = "L'item a été consommé!";
                    }

                    item.Quantite--;

                    if (item.Quantite == 0) //ajouter gestion des items d'utilité == 1 en utilisant l'exception CantRemoveForUtility
                    {
                        if (item.IdItemNavigation.Utilite == 1)
                            throw new CantRemoveForUtility();
                        else
                            DB.SacADos.Remove(item);
                    }
                    else
                        DB.SacADos.Update(item);

                    DB.Joueurs.Update(joueur);
                    DB.SaveChanges();
                }
                else
                    throw new ItemNotFound();

                return RedirectToAction("Index", "SacADos");
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn", "Accounts");
            }
            catch (ItemNotFound)
            {
                TempData["message"] = "Une erreur est survenue!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "Paniers");
            }
            catch (NotConsummable)
            {
                TempData["message"] = "Une erreur est survenue!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "Paniers");
            }
            catch (CantRemoveForUtility)
            {
                TempData["message"] = "L'item est trop utile, assurez-vous de garder au moins un fois cet item!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "SacADos");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vendre(int idItem, int quantite)
        {
            try
            {
                Joueur joueur = Session.GetConnected();
                var sac = DB.SacADos.Where(p => p.IdJoueur == joueur.IdJoueurs).Include(i => i.IdItemNavigation).ToList();
                if (sac != null)
                {
                    var item = sac.FirstOrDefault(i => i.IdItem == idItem);
                    if (item != null)
                    {
                        
                        item.Quantite -= quantite;
                        if (item.Quantite == 0) //ajouter gestion des items d'tilite == 1 en utilisant l'exception CantRemoveForUtility
                        {
                            if (item.IdItemNavigation.Utilite == 1)
                                throw new CantRemoveForUtility();
                            else
                                DB.SacADos.Remove(item);
                        }
                        else
                            DB.SacADos.Update(item);
                        TempData["message"] = "L'item a été vendu!";
                        var itemNav = item.IdItemNavigation;
                        joueur.Montant += (int)((float)itemNav.PrixUnitaire * Item.RetourSurVente * (float)quantite); ///un milliard de cast a changer
                        DB.Joueurs.Update(joueur);
                        itemNav.QuantiteStock += quantite;
                        DB.Items.Update(itemNav);
                        DB.SaveChanges();
                    }
                    else
                        throw new ItemNotFound();
                }
                else
                {
                    TempData["message"] = "Une erreur est survenue!";
                    TempData["isMessageBad"] = true;
                }
                return RedirectToAction("Index", "SacADos");
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn", "Accounts");
            }
            catch (ItemNotFound)
            {
                TempData["message"] = "Une erreur est survenue!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "Paniers");
            }
            catch (CantRemoveForUtility)
            {
                TempData["message"] = "L'item est trop utile, assurez-vous de garder au moins un fois cet item!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "SacADos");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Retirer(int idItem,int quantite)
        {
            try
            {
                Joueur joueur = Session.GetConnected();
                var sac = DB.SacADos.Include(i => i.IdItemNavigation).Where(p => p.IdJoueur == joueur.IdJoueurs).ToList();
                if (sac != null)
                {
                    var item = sac.FirstOrDefault(i => i.IdItem == idItem);
                    if (item != null)
                    {
                        
                        item.Quantite -= quantite;
                        if (item.Quantite == 0) //ajouter gestion des items d'tilite == 1 en utilisant l'exception CantRemoveForUtility
                        {
                            if (item.IdItemNavigation.Utilite == 1)
                                throw new CantRemoveForUtility();
                            else
                                DB.SacADos.Remove(item);
                        }
                        else
                            DB.SacADos.Update(item);
                        TempData["message"] = "L'item a été jeté!";
                        DB.SaveChanges();
                    }
                    else
                        throw new ItemNotFound();
                }
                else
                {
                    TempData["message"] = "Une erreur est survenue!";
                    TempData["isMessageBad"] = true;
                }
                return RedirectToAction("Index", "SacADos");
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn", "Accounts");
            }
            catch (ItemNotFound)
            {
                TempData["message"] = "Une erreur est survenue!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "Paniers");
            }
            catch (CantRemoveForUtility)
            {
                TempData["message"] = "L'item est trop utile, assurez-vous de garder au moins un fois cet item!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "SacADos");
            }
        }
    }
}
