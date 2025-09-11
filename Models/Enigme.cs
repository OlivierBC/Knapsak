using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Knapsak_CFTW.Models;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Enigme
    {
        [NotMapped]
        public static int StreakBonus = 500;
        public Enigme()
        {
            Reponses = new HashSet<Reponse>();
            Statistiques = new HashSet<Statistique>();
        }

        public int IdEnigmes { get; set; }
        public string Question { get; set; }
        public byte Difficulte { get; set; }
        public short EstActif { get; set; }

        public virtual Difficulte DifficulteNavigation { get; set; }
        public virtual ICollection<Reponse> Reponses { get; set; }
        public virtual ICollection<Statistique> Statistiques { get; set; }
        public string GetCssForDifficulty() => GetCssForDifficulty(Difficulte);
        public static string GetCssForDifficulty(int diff)
        {
            switch (diff)
            {
                case 1:
                    return "diff-facile";
                case 2:
                    return "diff-moyenne";
                case 3:
                    return "diff-difficile";
                default:
                    return "diff-unknown";
            }
        }
    }
}
