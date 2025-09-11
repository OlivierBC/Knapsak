using System;
using System.Collections.Generic;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Statistique
    {
        public int IdJoueur { get; set; }
        public int IdEnigme { get; set; }
        public short NbReussi { get; set; }
        public short NbRate { get; set; }
        public Statistique(int idJoueur,int idEnigme)
        {
            IdJoueur = idJoueur;
            IdEnigme = idEnigme;
            NbRate = 0;
            NbReussi = 0;
        }
        public virtual Enigme IdEnigmeNavigation { get; set; }
        public virtual Joueur IdJoueurNavigation { get; set; }
    }
}
