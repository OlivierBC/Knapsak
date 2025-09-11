using System;
using System.Collections.Generic;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Nourriture
    {
        public int IdItem { get; set; }
        public int? Calories { get; set; }
        public string ComposantNutritif { get; set; }
        public string Mineral { get; set; }
        public short? GainDeVie { get; set; }

        public virtual Item IdItemNavigation { get; set; }
    }
}
