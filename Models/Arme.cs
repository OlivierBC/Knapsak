using System;
using System.Collections.Generic;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Arme
    {
        public int IdItem { get; set; }
        public int Efficacite { get; set; }
        public string Genre { get; set; }
        public int? IdMunitions { get; set; }

        public virtual Item IdItemNavigation { get; set; }
        public virtual Munition IdMunitionsNavigation { get; set; }
    }
}
