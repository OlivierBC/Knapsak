using System.ComponentModel.DataAnnotations;
using Knapsak_CFTW.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Knapsak_CFTW.ViewModels
{
    public class ConnexionViewModel
    {
        [Remote("AliasNotExist", "Accounts", HttpMethod = "POST", ErrorMessage = "Alias invalide.")]
        [Display(Name = "Alias"), Required(ErrorMessage = "Un alias est requis.")]
        public string Alias { get; set; }

        [Display(Name = "Mot de passe"), Required(ErrorMessage = "Obligatoire")]
        [DataType(DataType.Password)]
        public string MPasse { get; set; }
    }
}
