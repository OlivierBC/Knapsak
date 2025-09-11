using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class SacADos
    {
        public int IdJoueur { get; set; }
        public int IdItem { get; set; }
        public int Quantite { get; set; }
        [NotMapped]
        public short? gainVie;

        public virtual Item IdItemNavigation { get; set; }
        public virtual Joueur IdJoueurNavigation { get; set; }
        public string ItemNom => IdItemNavigation?.Nom;
        public decimal ItemPrix => IdItemNavigation?.PrixUnitaire ?? 0;
        public decimal ItemPoids => IdItemNavigation?.Poids ?? 0;
        public string JoueurNom => IdJoueurNavigation?.Nom;
    }
}
