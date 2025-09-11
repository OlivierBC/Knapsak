using System;
using System.Collections.Generic;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Armure
    {
        public int IdItem { get; set; }
        public string Matiere { get; set; }
        public string Taille { get; set; }

        public virtual Item IdItemNavigation { get; set; }
    }
}
