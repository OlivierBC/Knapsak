using System;
using System.Collections.Generic;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Munition
    {
        public Munition()
        {
            Armes = new HashSet<Arme>();
        }

        public int IdItem { get; set; }
        public string Calibre { get; set; }

        public virtual Item IdItemNavigation { get; set; }
        public virtual ICollection<Arme> Armes { get; set; }
    }
}
