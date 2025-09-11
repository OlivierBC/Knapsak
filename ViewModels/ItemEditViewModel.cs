using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Knapsak_CFTW.ViewModels
{
    public class ItemEditViewModel
    {
        public int IdItems { get; set; }
        [Required]
        public string Nom { get; set; }
        public int QuantiteStock { get; set; }
        [Required]
        public string TypeItem { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal? PrixUnitaire { get; set; }
        public short? Poids { get; set; }
        public short Utilite { get; set; }
        [Required]
        public string LienImage { get; set; }
        public short FlagDispo { get; set; }
        public List<SelectListItem> AvailableTypes { get; set; }

        public ItemEditViewModel()
        {
            Nom = "";
            TypeItem = "Armes";
            Description = "";
            LienImage = "";
            FlagDispo = 0;
            AvailableTypes = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Armes", Value = "Armes" },
                    new SelectListItem { Text = "Medicaments", Value = "Medicaments" },
                    new SelectListItem { Text = "Armures", Value = "Armures" },
                    new SelectListItem { Text = "Nourritures", Value = "Nourritures" },
                    new SelectListItem { Text = "Munitions", Value = "Munitions" }
                };
        }
    }
}