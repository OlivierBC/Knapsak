namespace Knapsak_CFTW.Models
{
    public class Requetes
    {
        public int IdRequete { get; set; }
        public int IdJoueurs { get; set; }
        public int CapsulesDemandes { get; set; }
        public bool EstReglee { get; set; }

        public virtual Joueur Joueur { get; set; }
    }
}
