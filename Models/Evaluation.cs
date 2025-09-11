using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Evaluation
    {
        public const short MaxEtoiles = 5;
        public const int MaxCommLength = 300;
        public int IdJoueur { get; set; }
        public int IdItem { get; set; }
        public short NbEtoiles { get; set; }
        public string Commentaire { get; set; }

        public virtual Item IdItemNavigation { get; set; }
        public virtual Joueur IdJoueurNavigation { get; set; }

    }
}
