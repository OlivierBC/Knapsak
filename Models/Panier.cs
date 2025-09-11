using System;
using System.Collections.Generic;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Panier
    {
        public int IdJoueur { get; set; }
        public int IdItem { get; set; }
        public int Quantite { get; set; }

        public virtual Item IdItemNavigation { get; set; }
        public virtual Joueur IdJoueurNavigation { get; set; }

        public string ItemNom => IdItemNavigation?.Nom;
        public decimal ItemPrix => IdItemNavigation?.PrixUnitaire ?? 0;
        public decimal ItemPoids => IdItemNavigation?.Poids ?? 0;
        public string JoueurNom => IdJoueurNavigation?.Nom;

        public decimal TotalPrix => ItemPrix * Quantite; 
        public decimal TotalPoids => ItemPoids * Quantite;
    }
    class NotAvailableException : Exception { }
}
