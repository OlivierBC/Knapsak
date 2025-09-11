using Knapsak_CFTW.Models;
using System.ComponentModel.DataAnnotations;

namespace Knapsak_CFTW.ViewModels
{
    public class EnigmeEditViewModel
    {
        public int IdEnigmes { get; set; }
        [Required]
        public string Question { get; set; }
        [Required]
        public byte Difficulte { get; set; }
        public int? IndexBonneReponse { get; set; }
        public short EstActif { get; set; }
        public virtual Difficulte? DifficulteNavigation { get; set; }
        public List<ReponseEditViewModel> Reponses { get; set; } = new();
        public EnigmeEditViewModel()
        {
            Question = "";
            Difficulte = 1;
            EstActif = 0;
        }
    }
}
