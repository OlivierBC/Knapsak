using System;
using System.Collections.Generic;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Difficulte
    {
        public Difficulte()
        {
            Enigmes = new HashSet<Enigme>();
        }

        public byte IdDifficulte { get; set; }
        public short GainCaps { get; set; }
        public string Nom { get; set; }
        public short PerteVie { get; set; }

        public virtual ICollection<Enigme> Enigmes { get; set; }
    }
}
