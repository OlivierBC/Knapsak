using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Knapsak_CFTW.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Knapsak_CFTW.Controllers
{
    public class PaniersController : Controller
    {
        protected readonly ApplicationDbContext DB;
        private readonly SessionService Session;
        public PaniersController(ApplicationDbContext db, SessionService session)
        {
            DB = db;
            Session = session;
        }
        public ActionResult Index()
        {
            try
            {
                Joueur j = Session.GetConnected();

                var panier = DB.Paniers
                   .Where(p => p.IdJoueur == j.IdJoueurs)
                   .Select(p => new Panier
                   {
                       IdItem = p.IdItem,
                       Quantite = p.Quantite,
                       IdItemNavigation = p.IdItemNavigation,
                       IdJoueurNavigation = p.IdJoueurNavigation
                   })
                   .ToList();

                return View(panier);
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn", "Accounts");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ajouter(int idItem)
        {
            try
            {
                Joueur joueur = Session.GetConnected();
                var panier = DB.Paniers.FirstOrDefault(p => p.IdJoueur == joueur.IdJoueurs && p.IdItem == idItem);
                var item = Session.GetItem(idItem);
                if (panier != null)
                {
                    panier.Quantite += 1;
                    Session.CheckItemAvailability(item, panier.Quantite);
                    DB.Paniers.Update(panier);
                }
                else
                {
                    panier = new Panier
                    {
                        IdJoueur = joueur.IdJoueurs,
                        IdItem = idItem,
                        Quantite = 1,
                    };
                    Session.CheckItemAvailability(item, panier.Quantite);
                    DB.Paniers.Add(panier);
                }
                DB.SaveChanges();
                TempData["message"] = "L'item a été ajouté au panier!";
                return RedirectToAction("Index", "Items");
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn", "Accounts");
            }
            catch (NotAvailableException)
            {
                TempData["message"] = "L'item n'a pas été ajouté au panier!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "Items");
            }          
            catch (ItemNotFound)
            {
                TempData["message"] = "Cet item n'existe pas!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "Items");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Retirer(int idItem)
        {
            try
            {
                Joueur joueur = Session.GetConnected();
                var panier = DB.Paniers.Where(p => p.IdJoueur == joueur.IdJoueurs).ToList();
                if (panier != null)
                {
                    var item = panier.FirstOrDefault(i=> i.IdItem == idItem);
                    if (item != null)
                    {
                        TempData["message"] = "L'item a été retiré du panier!";
                        DB.Paniers.Remove(item);
                        DB.SaveChanges();
                    }
                    else
                    {
                        TempData["message"] = "Une erreur est survenue!";
                        TempData["isMessageBad"] = true;
                    }
                } 
                else
                {
                    TempData["message"] = "Une erreur est survenue!";
                    TempData["isMessageBad"] = true;
                }
                return RedirectToAction("Index", "Paniers");
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn", "Accounts");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Modifier(int idItem, int quantite)
        {
            try
            {
                Joueur joueur = Session.GetConnected();
                var item = Session.GetItem(idItem);
                Session.CheckItemAvailability(item, quantite);
                var panier = DB.Paniers.FirstOrDefault(p => p.IdJoueur == joueur.IdJoueurs && p.IdItem == idItem);
                if (panier != null)
                {
                    panier.Quantite = quantite;
                    DB.Paniers.Update(panier);
                    DB.SaveChanges();
                }
                return RedirectToAction("Index", "Paniers");
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn", "Accounts");
            }
            catch (NotAvailableException)
            {
                TempData["message"] = "La quantité de l'item n'a pas été modifier!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "Paniers");
            }
            catch (ItemNotFound)
            {
                TempData["message"] = "Cet item n'existe pas!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "Paniers");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vider()
        {
            try
            {
                Joueur joueur = Session.GetConnected();
                var panier = DB.Paniers.Where(p => p.IdJoueur == joueur.IdJoueurs);
                if (!panier.Any())
                {
                    TempData["message"] = "Votre panier est vide !";
                    return RedirectToAction("Index", "Paniers");
                }
                if (panier.ToList().Count() > 0)
                {
                    TempData["message"] = "Votre panier a été vidé!";
                    DB.Paniers.RemoveRange(panier);
                    DB.SaveChanges();
                }
                return RedirectToAction("Index", "Paniers");
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn", "Accounts");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payer()
        {
            try
            {
                Joueur joueur = Session.GetConnected();
                var panier = DB.Paniers
                    .Where(p => p.IdJoueur == joueur.IdJoueurs)
                    .Include(p => p.IdItemNavigation)
                    .ToList();

                if (!panier.Any())
                {
                    TempData["message"] = "Votre panier est vide !";
                    return RedirectToAction("Index", "Paniers");
                }

                int totalPrix = (int)panier.Sum(p => p.IdItemNavigation.PrixUnitaire * p.Quantite);
                short totalPoids = (short)panier.Sum(p => p.IdItemNavigation.Poids * p.Quantite);

                if (joueur.Montant < totalPrix)
                {
                    TempData["message"] = "Vous n'avez pas assez de caps pour payer votre panier !";
                    TempData["isMessageBad"] = true;
                    return RedirectToAction("Index", "Paniers");
                }

                joueur.Montant -= totalPrix;

                joueur.PoidsTotal += totalPoids;

                DB.Joueurs.Update(joueur);

                foreach (var itemPanier in panier)
                {
                    var sac = DB.SacADos.FirstOrDefault(s => s.IdJoueur == joueur.IdJoueurs && s.IdItem == itemPanier.IdItem);

                    if (sac != null)
                    {
                        sac.Quantite += itemPanier.Quantite;
                        DB.SacADos.Update(sac); 
                    }
                    else
                    {
                        joueur.SacADos.Add(new SacADos
                        {
                            IdJoueur = joueur.IdJoueurs,
                            IdItem = itemPanier.IdItem,
                            Quantite = itemPanier.Quantite
                        });
                    }

                    var item = itemPanier.IdItemNavigation; 
                    item.QuantiteStock -= itemPanier.Quantite;
                    if (item.QuantiteStock < 0)
                    {
                        throw new NotAvailableException();
                    }
                    DB.Items.Update(item);
                }

                DB.Paniers.RemoveRange(panier);
                DB.SaveChanges();
                TempData["message"] = "Votre paiement a été effectué avec succès !";
                return RedirectToAction("Index", "Paniers");
            }
            catch (NotConnectedException)
            {
                return RedirectToAction("SignIn", "Accounts");
            }
            catch (NotAvailableException)
            {
                TempData["message"] = "Certains items ne sont plus disponible à l'achat!";
                TempData["isMessageBad"] = true;
                return RedirectToAction("Index", "Paniers");
            }
        }
    }
}