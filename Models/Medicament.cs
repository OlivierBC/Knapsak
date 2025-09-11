using System;
using System.Collections.Generic;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Medicament
    {
        public int IdItem { get; set; }
        public string Effet { get; set; }
        public int? Duree { get; set; }
        public string EffetIndesirable { get; set; }
        public short? GainDeVie { get; set; }

        public virtual Item IdItemNavigation { get; set; }
    }
}
